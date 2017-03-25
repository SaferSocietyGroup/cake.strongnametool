using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;

namespace Cake.StrongNameTool
{
    /// <summary>
    /// Strong name resolver.
    /// </summary>
    public sealed class StrongNameResolver : IStrongNameToolResolver
    {
        private readonly IFileSystem _fileSystem;
        private readonly ICakeEnvironment _environment;
        private readonly IRegistry _registry;
        private FilePath _strongnameToolPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="Cake.StrongNameTool.StrongNameResolver"/> class.
        /// </summary>
        /// <param name="fileSystem">The filesystem.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="registry">The registry.</param>
        public StrongNameResolver(IFileSystem fileSystem, ICakeEnvironment environment, IRegistry registry)
        {
            _fileSystem = fileSystem;
            _environment = environment;
            _registry = registry;

            if (fileSystem == null)
            {
                throw new ArgumentNullException("fileSystem");
            }
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }
            if (environment == null)
            {
                throw new ArgumentNullException("environment");
            }

        }

        /// <summary>
        /// Resolves the path to the strong name tool (sn.exe)
        /// </summary>
        /// <returns>The path to sn.exe</returns>
        public FilePath GetPath()
        {
            if (_strongnameToolPath != null)
            {
                return _strongnameToolPath;
            }

            _strongnameToolPath = GetFromDisc() ?? GetFromRegistry();
            if (_strongnameToolPath == null)
            {
                throw new CakeException("Failed to find sn.exe.");
            }

            return _strongnameToolPath;
        }

        /// <summary>
        /// Gets the path to sn.exe from disc.
        /// </summary>
        /// <returns>The path to sn.exe from disc.</returns>
        private FilePath GetFromDisc()
        {
            // Get the path to program files.
            var programFilesPath = _environment.GetSpecialPath(SpecialPath.ProgramFilesX86);

            var possibleVersions = new[] { "v10.0A", "v8.1A", "v8.1", "v8.0", "v7.0A" };

            // Get a list of the files we should check.
            var files = new List<FilePath>();
            if (_environment.Platform.Is64Bit)
            {
                // 64-bit specific paths.
                foreach (var version in possibleVersions)
                {
                    //NETFX4
                    files.Add(programFilesPath.Combine(string.Format(@"Microsoft SDKs\Windows\{0}\Bin\NETFX 4.0 Tools\x64\", version)).CombineWithFilePath("sn.exe"));
                    files.Add(programFilesPath.Combine(string.Format(@"Microsoft SDKs\Windows\{0}\Bin\x64\", version)).CombineWithFilePath("sn.exe"));
                }
            }
            else
            {
                // 32-bit specific paths.
                foreach (var version in possibleVersions)
                {
                    //NETFX4
                    files.Add(programFilesPath.Combine(string.Format(@"Microsoft SDKs\Windows\{0}\Bin\NETFX 4.0 Tools\", version)).CombineWithFilePath("sn.exe"));
                    files.Add(programFilesPath.Combine(string.Format(@"Microsoft SDKs\Windows\{0}\Bin\", version)).CombineWithFilePath("sn.exe"));
                }
            }

            // Return the first path that exist.
            return files.FirstOrDefault(file => _fileSystem.Exist(file));
        }

        /// <summary>
        /// Gets the installation folder of sn.exe from the registry.
        /// </summary>
        /// <returns>The install folder to sn.exe from registry.</returns>
        private FilePath GetFromRegistry()
        {
            using (var root = _registry.LocalMachine.OpenKey("Software\\Microsoft\\Microsoft SDKs\\Windows"))
            {
                if (root == null)
                {
                    return null;
                }

                var keyName = root.GetSubKeyNames();
                foreach (var key in keyName)
                {
                    var sdkKey = root.OpenKey(key);
                    if (sdkKey != null)
                    {
                        IRegistryKey fxKey;
                        if (_environment.Platform.Is64Bit)
                        {
                            fxKey = sdkKey.OpenKey("WinSDK-NetFx40Tools-x64");
                        }
                        else
                        {
                            fxKey = sdkKey.OpenKey("WinSDK-NetFx40Tools");
                        }

                        if (fxKey != null)
                        {
                            var installationFolder = fxKey.GetValue("InstallationFolder") as string;
                            if (!string.IsNullOrWhiteSpace(installationFolder))
                            {
                                var installationPath = new DirectoryPath(installationFolder);
                                var signToolPath = installationPath.CombineWithFilePath("sn.exe");

                                if (_fileSystem.Exist(signToolPath))
                                {
                                    return signToolPath;
                                }
                            }
                        }
                        else
                        {
                            // if NETFX4 isn't present
                            var installationFolder = sdkKey.GetValue("CurrentInstallFolder") as string;
                            if (!string.IsNullOrEmpty(installationFolder))
                            {
                                var installationPath = new DirectoryPath(installationFolder);
                                var signToolPath = installationPath.CombineWithFilePath("sn.exe");

                                if (_fileSystem.Exist(signToolPath))
                                {
                                    return signToolPath;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}

