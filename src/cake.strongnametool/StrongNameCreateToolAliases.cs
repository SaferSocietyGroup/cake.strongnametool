using System;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;

namespace Cake.StrongNameTool
{
    /// <summary>
    /// Strong Name (sn.exe) tool aliases.static It is possible to create a new snk file
    /// by passing the path to the file that you want to create.
    /// </summary>
    [CakeAliasCategoryAttribute("Strong Naming")]
    public static class StrongNameCreateToolAliases
    {
        /// <summary>
        /// Uses sn.exe to create a new strong name key file.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="strongNameKeyFilePath">The path for the new strong name key file.</param>
        /// <example>
        /// <code>
        /// Task("Create-Strong-Key-File")
        ///     .Does(() =>
        /// {
        ///     var file = "test.snk";
        ///     StrongNameReSign(file);
        /// });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        public static void StrongNameCreate(this ICakeContext context, FilePath strongNameKeyFilePath)
        {
            if (strongNameKeyFilePath == null)
            {
                throw new ArgumentNullException("strongNameKeyFilePath");
            }

            var runner = new StrongNameToolRunner(context.FileSystem, context.Environment, context.ProcessRunner, context.Tools, context.Registry);
            runner.Run(strongNameKeyFilePath);
        }
    }
}
