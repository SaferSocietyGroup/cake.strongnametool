using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;

namespace Cake.StrongNameTool
{
    /// <summary>
    /// Strong name verify tool aliases.
    /// </summary>
    [CakeAliasCategoryAttribute("Strong Naming")]
    public static class StrongNameVerifyToolAliases
    {

        /// <summary>
        /// Strongs the name verify.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="assembly">Assembly.</param>
        /// <param name="settings">Settings.</param>
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
        /// Strongs the name verify.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="assembly">Assembly.</param>
        /// <param name="settings">Settings.</param>
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
        /// Strongs the name verify.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="assemblies">Assemblies.</param>
        /// <param name="settings">Settings.</param>
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
        /// Strongs the name verify.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="assemblies">Assemblies.</param>
        /// <param name="settings">Settings.</param>
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
