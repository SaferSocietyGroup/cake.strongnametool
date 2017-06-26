using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;

namespace Cake.StrongNameTool
{
    /// <summary>
    /// Strong Name (sn.exe) tool aliases. It is possible to resign a delay-signed assembly. 
    /// The resign alias uses the sn.exe containers to resign the specified assemblies.
    /// the aliases also provide verification functionality. The verify alias will check if an assembly has a 
    /// strong name or not.
    /// <see cref="Cake.StrongNameTool.StrongNameToolSettings.ForceVerification"/>  set to true.
    /// </summary>
    [CakeAliasCategoryAttribute("Strong Naming")]
    public static class StrongNameReSignToolAliases
    {
        /// <summary>
        /// Uses sn.exe to resign the specified assembly.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="assembly">The target assembly.</param>
        /// <param name="settings">The Strong Name tool settings to use.</param>
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
        /// Uses sn.exe to resign the specified assembly.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="assembly">The target assembly to resign.</param>
        /// <param name="settings">The Strong Name tool settings to use.</param>
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
        /// Uses sn.exe to resign the specified assemblies.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="assemblies">The assemblies to resign.</param>
        /// <param name="settings">The Strong Name tool settings to use.</param>
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
        /// Uses sn.exe to resign the specified assemblies.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="assemblies">The assemblies to resign.</param>
        /// <param name="settings">The Strong Name tool settings to use.</param>
        /// <example>
        /// <code>
        /// Task("Verify")
        ///     .IsDependentOn("Clean")
        ///     .IsDependentOn("Restore")
        ///     .IsDependentOn("Build")
        ///     .Does(() =>
        /// {
        ///     var files = GetFiles("*.dll");
        ///     StrongNameReSign(file, new StrongNameToolSettings {
        ///             Container = "YOUR_CONTAINER_NAME" 
        ///     });
        /// });
        /// </code>
        /// </example>
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

            var runner = new StrongNameToolRunner(context.FileSystem, context.Environment, context.ProcessRunner, context.Tools, context.Registry);
            foreach (var assembly in assemblies)
            {
                runner.Run("resign", assembly, settings);
            }
        }
    }
}
