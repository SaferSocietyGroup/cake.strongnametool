using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;

namespace Cake.StrongNameTool
{
    /// <summary>
    /// Strong name resign tool aliases.
    /// </summary>
    [CakeAliasCategoryAttribute("Strong Naming")]
    public static class StrongNameReSignToolAliases
    {
        /// <summary>
        /// Resigns the specified assembly.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="assembly">The target assembly.</param>
        /// <param name="settings">The settings.</param>
        /// <example>
        /// <code>
        /// Task("Resign")
        ///     .IsDependentOn("Clean")
        ///     .IsDependentOn("Restore")
        ///     .IsDependentOn("Build")
        ///     .Does(() =>
        /// {
        ///     var file = "Core.dll";
        ///     StrongNameReSign(file, new StrongNameToolSettings {
        ///             Container = "YOUR_CONTAINER_NAME"
        ///     });
        /// });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void StrongNameReSign(this ICakeContext context, string assembly, StrongNameToolSettings settings)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }
            StrongNameReSign(context, new FilePath(assembly), settings);
        }

        /// <summary>
        /// Resigns the specified assembly.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="assembly">The target assembly.</param>
        /// <param name="settings">The settings.</param>
        /// <example>
        /// <code>
        /// Task("Resign")
        ///     .IsDependentOn("Clean")
        ///     .IsDependentOn("Restore")
        ///     .IsDependentOn("Build")
        ///     .Does(() =>
        /// {
        ///     var file = new FilePath("Core.dll");
        ///     StrongNameReSign(file, new StrongNameToolSettings {
        ///             Container = "YOUR_CONTAINER_NAME"
        ///     });
        /// });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void StrongNameReSign(this ICakeContext context, FilePath assembly, StrongNameToolSettings settings)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }
            var paths = new[] { assembly };
            StrongNameReSign(context, paths, settings);
        }

        /// <summary>
        /// Resigns the specified assembly.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="assemblies">The target assembly.</param>
        /// <param name="settings">The settings.</param>
        /// <example>
        /// <code>
        /// Task("Resign")
        ///     .IsDependentOn("Clean")
        ///     .IsDependentOn("Restore")
        ///     .IsDependentOn("Build")
        ///     .Does(() =>
        /// {
        ///     var file = new string[] { "Core.dll", "Common.dll" };
        ///     StrongNameReSign(file, new StrongNameToolSettings {
        ///             Container = "YOUR_CONTAINER_NAME"
        ///     });
        /// });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void StrongNameReSign(this ICakeContext context, IEnumerable<string> assemblies, StrongNameToolSettings settings)
        {
            if (assemblies == null)
            {
                throw new ArgumentNullException("assemblies");
            }
            var paths = assemblies.Select(p => new FilePath(p));
            StrongNameReSign(context, paths, settings);
        }

        /// <summary>
        /// Resigns the specified assembly.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="assemblies">Assemblies.</param>
        /// <param name="settings">Settings.</param>
        [CakeMethodAlias]
        public static void StrongNameReSign(this ICakeContext context, IEnumerable<FilePath> assemblies, StrongNameToolSettings settings)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (assemblies == null)
            {
                throw new ArgumentNullException("assemblies");
            }
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            var runner = new StrongNameToolRunner(context.FileSystem, context.Environment, context.ProcessRunner, context.Globber, context.Registry);
            foreach (var assembly in assemblies)
            {
                runner.Run("resign", assembly, settings);
            }
        }
    }
}
