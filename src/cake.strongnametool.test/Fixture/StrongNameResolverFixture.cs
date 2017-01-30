using Cake.StrongNameTool;
using Cake.Core;
using Cake.Core.IO;
using Cake.Testing;
using NSubstitute;


namespace Cake.StrongNameTool.Test.Fixture
{
    internal sealed class StrongNameResolverFixture
    {
        private readonly bool _is64Bit;

        public IFileSystem FileSystem { get; set; }
        public FakeEnvironment Environment { get; set; }
        public IRegistry Registry { get; set; }


        public StrongNameResolverFixture(bool is64Bit = true)
        {
            _is64Bit = is64Bit;

            FileSystem = Substitute.For<IFileSystem>();
            Environment =new FakeEnvironment(PlatformFamily.Windows, _is64Bit);
            Environment.SetSpecialPath(SpecialPath.ProgramFiles, "/ProgramFiles");
            Environment.SetSpecialPath(SpecialPath.ProgramFilesX86, "/ProgramFilesX86");
            Registry = Substitute.For<IRegistry>();
        }

        public void GivenThatTargetFrameworkIs40()
        {
            Environment.SetTargetFramework(new System.Runtime.Versioning.FrameworkName(".NETFramework,Version=v4.0"));
        }

        public void GivenThatToolExistInKnownPath()
        {
            if (_is64Bit)
            {
                FileSystem.Exist(Arg.Is<FilePath>(p => p.FullPath == "/ProgramFilesX86/Microsoft SDKs/Windows/v7.0A/Bin/NETFX 4.0 Tools/x64/sn.exe")).Returns(true);
            }
            else
            {
                FileSystem.Exist(Arg.Is<FilePath>(p => p.FullPath == "/ProgramFilesX86/Microsoft SDKs/Windows/v7.0A/Bin/NETFX 4.0 Tools/sn.exe")).Returns(true);
            }
        }

        public void GivenThatToolHasRegistryKey()
        {
            var fxKey64 = Substitute.For<IRegistryKey>();
            fxKey64.GetValue("InstallationFolder").Returns("C:/Program Files (x86)/Microsoft SDKs/Windows/v7.0A/Bin/NETFX 4.0 Tools/x64/");
            var fxKey = Substitute.For<IRegistryKey>();
            fxKey.GetValue("InstallationFolder").Returns("C:/Program Files (x86)/Microsoft SDKs/Windows/v7.0A/Bin/NETFX 4.0 Tools/");

            var sdkKey = Substitute.For<IRegistryKey>();
            sdkKey.OpenKey("WinSDK-NetFx40Tools-x64").Returns(fxKey64);
            sdkKey.OpenKey("WinSDK-NetFx40Tools").Returns(fxKey);

            var windowsKey = Substitute.For<IRegistryKey>();
            windowsKey.GetSubKeyNames().Returns(new[] { "v7.0A" });
            windowsKey.OpenKey("v7.0A").Returns(sdkKey);

            var localMachine = Substitute.For<IRegistryKey>();
            localMachine.OpenKey("Software\\Microsoft\\Microsoft SDKs\\Windows").Returns(windowsKey);

            FileSystem.Exist(Arg.Is<FilePath>(p => p.FullPath == "C:/Program Files (x86)/Microsoft SDKs/Windows/v7.0A/Bin/NETFX 4.0 Tools/x64/sn.exe")).Returns(true);
            FileSystem.Exist(Arg.Is<FilePath>(p => p.FullPath == "C:/Program Files (x86)/Microsoft SDKs/Windows/v7.0A/Bin/NETFX 4.0 Tools/sn.exe")).Returns(true);

            Registry.LocalMachine.Returns(localMachine);
        }

        public void GivenThatNoSdkRegistryKeyExist()
        {
            var localMachine = Substitute.For<IRegistryKey>();
            localMachine.OpenKey("Software\\Microsoft\\Microsoft SDKs\\Windows").Returns((IRegistryKey)null);
            Registry.LocalMachine.Returns(localMachine);
        }

        public FilePath Resolve()
        {
            var resolver = new StrongNameResolver(FileSystem, Environment, Registry);
            return resolver.GetPath();
        }
    }
}
