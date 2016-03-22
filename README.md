# cake.strongnametool
### Example usage:
Some prerequisites are that you are on Windows and have installed one of the Windows SDK:s  
and have a project that is set to be `delay-sign` with some keypair `keypair.snk`.

Creating a container for the `keypair.snk`
`sn.exe -i <infile> <container>` Install key pair from <infile> into a key container named <container>

```cake
#addin nuget:?package=Cake.StrongNameTool

...

Task("StrongNameResign")
    .IsDependentOn("Build")
    .WithCriteria(isRunningOnWindows)
    .Does(() => {
   
    var asmGreet = GetFiles("./src/**/bin/" + configuration + "/greeter*.dll");
    
    StrongNameReSign(asmGreet, new StrongNameToolSettings()
    {
        Container = "Your-SNK-Container" 
    });
    
    StrongNameVerify(asmGreet, new StrongNameToolSettings());

});

```