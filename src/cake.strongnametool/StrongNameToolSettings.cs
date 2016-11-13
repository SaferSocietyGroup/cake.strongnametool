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
        /// <value>The name of the key container that contains your strong name keys.</value>
        public string Container { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to force verification. 
        /// </summary>
        /// <value><c>true</c> if you wish to force verification; otherwise, <c>false</c>.</value>
        public bool ForceVerification { get; set;}
    }
}
