//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Get whether or not this is a local build.
var local = BuildSystem.IsLocalBuild;
var isRunningOnUnix = IsRunningOnUnix();
var isRunningOnWindows = IsRunningOnWindows();
var isRunningOnBamboo = Bamboo.IsRunningOnBamboo;


// Parse release notes.
var releaseNotes = ParseReleaseNotes("./release-notes.md");

// Get version.
var buildNumber = Bamboo.Environment.Build.Number;
var version = releaseNotes.Version.ToString();
var semVersion = local ? version : (version + string.Concat("-build-", buildNumber));

// Define directories.
var buildDir = Directory("./src/cake.strongnametool/bin") + Directory(configuration);
var buildResultDir = Directory("./artifacts") + Directory("v" + semVersion);
var testResultsDir = buildResultDir + Directory("test-results");
var nugetRoot = buildResultDir + Directory("nuget");
var binDir = buildResultDir + Directory("bin");

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(context =>
{
    Information("Building version {0} of Cake.StrongNameTool.", semVersion);
});

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectories(new DirectoryPath[] {
        buildResultDir, binDir, testResultsDir, nugetRoot});
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("./cake.strongnametool.sln", new NuGetRestoreSettings {
        Source = new List<string> {
            "https://www.nuget.org/api/v2/"
        }
    });
});

Task("Patch-Assembly-Info")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    var file = "./solution-info.cs";
    CreateAssemblyInfo(file, new AssemblyInfoSettings {
        Product = "Cake.strongnametool",
        Version = version,
        FileVersion = version,
        InformationalVersion = semVersion,
        Copyright = "Copyright (c) Safer Society Group"
    });
});

Task("Build")
    .IsDependentOn("Patch-Assembly-Info")
    .Does(() =>
{
    if(isRunningOnUnix)
    {
        XBuild("./cake.strongnametool.sln", new XBuildSettings()
            .SetConfiguration(configuration)
            .WithProperty("POSIX", "True")
            .WithProperty("TreatWarningsAsErrors", "True")
            .SetVerbosity(Verbosity.Minimal)
        );
    }
    else
    {
        MSBuild("./cake.strongnametool.sln", new MSBuildSettings()
            .SetConfiguration(configuration)
            .WithProperty("Windows", "True")
            .WithProperty("TreatWarningsAsErrors", "True")
            .UseToolVersion(MSBuildToolVersion.NET45)
            .SetVerbosity(Verbosity.Minimal)
            .SetNodeReuse(false));
    }
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    XUnit2("./src/**/bin/" + configuration + "/*.test.dll", new XUnit2Settings {
        OutputDirectory = testResultsDir,
        XmlReportV1 = true,
        NoAppDomain = true
    });
});


Task("Copy-Files")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
{
    CopyFileToDirectory(buildDir + File("cake.strongnametool.dll"), binDir);
    CopyFileToDirectory(buildDir + File("cake.strongnametool.xml"), binDir);

    if(isRunningOnWindows)
    {
        CopyFileToDirectory(buildDir + File("cake.strongnametool.pdb"), binDir);
    }

    // Copy testing assemblies.
    var testingDir = Directory("./src/cake.strongnametool.test/bin") + Directory(configuration);
    CopyFileToDirectory(testingDir + File("cake.strongnametool.test.dll"), binDir);
    if(isRunningOnWindows)
    {
        CopyFileToDirectory(testingDir + File("cake.strongnametool.test.pdb"), binDir);
    }

    CopyFiles(new FilePath[] { "LICENSE", "README.md", "release-notes.md" }, binDir);

});

Task("Zip-Files")
    .IsDependentOn("Copy-Files")
    .Does(() =>
{

    var packageFile = File("cake.strongnametool-bin-v" + semVersion + ".zip");

    var packagePath = buildResultDir + packageFile;

    var files = GetFiles(binDir.Path.FullPath + "/*");

    Zip(binDir, packagePath, files);

});


Task("Create-NuGet-Packages")
    .IsDependentOn("Copy-Files")
    .WithCriteria(() => isRunningOnWindows)
    .Does(() =>
{
        NuGetPack("./nuspec/cake.strongnametool.nuspec", new NuGetPackSettings {
            Version = semVersion,
            ReleaseNotes = releaseNotes.Notes.ToArray(),
            BasePath = binDir,
            OutputDirectory = nugetRoot,
            Symbols = true,
            NoPackageAnalysis = true
        });

});




//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Package")
  .IsDependentOn("Zip-Files")
  .IsDependentOn("Create-NuGet-Packages");

Task("Default")
  .IsDependentOn("Package");

Task("Publish");
  //.IsDependentOn("Publish-MyGet");


Task("Travis")
  .IsDependentOn("Run-Unit-Tests");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
