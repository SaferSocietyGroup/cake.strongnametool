using System;
using Cake.Core.IO;
using Cake.Core.Tooling;

namespace Cake.StrongNameTool
{
    /// <summary>
    /// Contains the settings used by sn.exe
    /// </summary>
    public sealed class StrongNameToolSettings : ToolSettings
    {
        /// <summary>
        /// Container name
        /// </summary>
        public string Container { get; set; }
    }
}
