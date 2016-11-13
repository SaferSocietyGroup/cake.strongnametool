using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;

namespace Cake.StrongNameTool
{
    /// <summary>
    /// Strong name tool runner.
    /// </summary>
    public sealed class StrongNameToolRunner : Tool<StrongNameToolSettings>
    {
        private readonly IStrongNameToolResolver _resolver;
        private readonly IFileSystem _fileSystem;
        private readonly ICakeEnvironment _environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="Cake.StrongNameTool.StrongNameToolRunner"/> class.
        /// </summary>
        /// <param name="filesystem">The filesystem.</param>
        /// <param name="enviroment">The enviroment.</param>
        /// <param name="processrunner">The processrunner.</param>
        /// <param name="globber">The globber.</param>
        /// <param name="registry">The registry.</param>
        public StrongNameToolRunner(IFileSystem filesystem, ICakeEnvironment enviroment, IProcessRunner processrunner, IGlobber globber, IRegistry registry):this(filesystem, enviroment, processrunner, globber, registry, null)
        {
        }
        internal StrongNameToolRunner(IFileSystem fileSystem, ICakeEnvironment environment, IProcessRunner processRunner, IGlobber globber, IRegistry registry,IStrongNameToolResolver resolver):base(fileSystem, environment,processRunner, globber)
        {
            _fileSystem = fileSystem;
            _environment = environment;
            _resolver = resolver ?? new StrongNameResolver(fileSystem, environment, registry);
        }

        /// <summary>
        /// Run the specified command on files specified by assemblyPath and the settings.
        /// </summary>
        /// <param name="command">The command, verify or resign</param>
        /// <param name="assemblyPath">The assembly path.</param>
        /// <param name="settings">The settings.</param>
        public void Run(String command, FilePath assemblyPath, StrongNameToolSettings settings)
        {
            if (assemblyPath == null)
            {
                throw new ArgumentNullException("assemblyPath");
            }
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            if (assemblyPath.IsRelative)
            {
                assemblyPath = assemblyPath.MakeAbsolute(_environment);
            }

            Run(settings, GetArguments(command, assemblyPath, settings));
        }

        /// <summary>
        /// Gets the arguments based on command and settings.
        /// </summary>
        /// <returns>The arguments.</returns>
        /// <param name="command">The command, verify or resign</param>
        /// <param name="assemblyPath">The assembly path.</param>
        /// <param name="settings">The settings.</param>
        private ProcessArgumentBuilder GetArguments(String command, FilePath assemblyPath, StrongNameToolSettings settings)
        {
            if (!_fileSystem.Exist(assemblyPath))
            {
                const string format = "{0}: The assembly '{1}' do not exist.";
                var message = string.Format(CultureInfo.InvariantCulture, format, GetToolName(), assemblyPath.FullPath);
                throw new CakeException(message);
            }

            ProcessArgumentBuilder builder = new ProcessArgumentBuilder();

            if(command.Equals("verify"))
            {
                // verify
                if (settings.ForceVerification) {
                    builder.Append ("-vf");
                } 
                else {
                    builder.Append ("-v");
                }

                // Target Assembly to verify.
                builder.AppendQuoted(assemblyPath.MakeAbsolute(_environment).FullPath);
            }
            else if(command.Equals("resign"))
            {
                if (string.IsNullOrEmpty(settings.Container))
                {
                    const string format = "{0}: Container is required but not specified.";
                    var message = string.Format(CultureInfo.InvariantCulture, format, GetToolName());
                    throw new CakeException(message);
                }

                // Resign
                builder.Append("-Rca");
                // Target Assembly to resign.
                builder.AppendQuoted(assemblyPath.MakeAbsolute(_environment).FullPath);
                // The container name
                builder.Append(settings.Container);
            }

            return builder;
        }

        /// <summary>
        /// Gets the name of the tool.
        /// </summary>
        /// <returns>The name of the tool.</returns>
        protected override string GetToolName()
        {
            return  "sn";
        }

        /// <summary>
        /// Gets the possible names of the tool executable.
        /// </summary>
        /// <returns>The tool executable name.</returns>
        protected override IEnumerable<string> GetToolExecutableNames()
        {
            return new[] { "sn.exe" };
        }

        /// <summary>
        /// Gets alternative file paths which the tool may exist in
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns>The default tool path.</returns>
        protected override IEnumerable<FilePath> GetAlternativeToolPaths(StrongNameToolSettings settings)
        {
            var path = _resolver.GetPath();
            return path != null
                ? new[] { path }
                : Enumerable.Empty<FilePath>();
        }
    }
}
