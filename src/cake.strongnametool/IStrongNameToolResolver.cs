using Cake.Core.IO;

namespace Cake.StrongNameTool
{
    /// <summary>
    /// Represents a strong name tool resolver.
    /// </summary>
    public interface IStrongNameToolResolver
    {
        /// <summary>
        /// Resolves the path to the strong name tool (sn.exe)
        /// </summary>
        /// <returns>The path to sn.exe</returns>
        FilePath GetPath();
    }
}
