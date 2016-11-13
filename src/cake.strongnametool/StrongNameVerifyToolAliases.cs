using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;

namespace Cake.StrongNameTool
{
    /// <summary>
    /// Strong name verify tool aliases. If one has turned off strong name verification on a machine 
    /// you can provide the StrongNameToolSettings with ForceVerification set to true.
    /// </summary>
    [CakeAliasCategoryAttribute("Strong Naming")]
    public static class StrongNameVerifyToolAliases
    {

        /// <summary>
        /// Verify assembly for strong name signature self consistency. 
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="assembly">The assembly to verify.</param>
        /// <param name="settings">The Strong Name tool settings to use.</param>
        /// <example>
        /// <code>
        /// Task("Verify")
        ///     .IsDependentOn("Clean")
        ///     .IsDependentOn("Restore")
        ///     .IsDependentOn("Build")
        ///     .Does(() =>
        /// {
        ///     var file = "Core.dll";
        ///     StrongNameVerify(file, new StrongNameToolSettings {
        ///             ForceVerification = true
        ///     });
        /// });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void StrongNameVerify(this ICakeContext context, string assembly, StrongNameToolSettings settings)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }
            StrongNameVerify(context, new FilePath(assembly), settings);
        }

        /// <summary>
        /// Verify assembly for strong name signature self consistency. 
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="assembly">The assembly to verify.</param>
        /// <param name="settings">The Strong Name tool settings to use.</param>
        /// <example>
        /// <code>
        /// Task("Verify")
        ///     .IsDependentOn("Clean")
        ///     .IsDependentOn("Restore")
        ///     .IsDependentOn("Build")
        ///     .Does(() =>
        /// {
        ///     var file = new FilePath("Core.dll");
        ///     StrongNameVerify(file, new StrongNameToolSettings {
        ///             ForceVerification = true
        ///     });
        /// });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void StrongNameVerify(this ICakeContext context, FilePath assembly, StrongNameToolSettings settings)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }
            var paths = new[] { assembly };
            StrongNameVerify(context, paths, settings);
        }

        /// <summary>
        /// Verify assembly for strong name signature self consistency.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="assemblies">The assemblies to verify.</param>
        /// <param name="settings">The Strong Name tool settings to use.</param>
        /// <example>
        /// <code>
        /// Task("Verify")
        ///     .IsDependentOn("Clean")
        ///     .IsDependentOn("Restore")
        ///     .IsDependentOn("Build")
        ///     .Does(() =>
        /// {
        ///     var file = new string[] { "Core.dll", "Common.dll" };
        ///     StrongNameVerify(file, new StrongNameToolSettings {
        ///             ForceVerification = true
        ///     });
        /// });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void StrongNameVerify(this ICakeContext context, IEnumerable<string> assemblies, StrongNameToolSettings settings)
        {
            if (assemblies == null)
            {
                throw new ArgumentNullException("assemblies");
            }
            var paths = assemblies.Select(p => new FilePath(p));
            StrongNameVerify(context, paths, settings);
        }

        /// <summary>
        /// Verify assembly for strong name signature self consistency. 
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="assemblies">The assemblies to verify.</param>
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
        ///     StrongNameVerify(file, new StrongNameToolSettings {
        ///             ForceVerification = true
        ///     });
        /// });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void StrongNameVerify(this ICakeContext context, IEnumerable<FilePath> assemblies, StrongNameToolSettings settings)
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
                runner.Run("verify", assembly, settings);
            }
        }
    }
}
