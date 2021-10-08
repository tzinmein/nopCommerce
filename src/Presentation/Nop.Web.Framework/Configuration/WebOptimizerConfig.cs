using System.Text.Json.Serialization;
using Nop.Core.Configuration;
using WebOptimizer;

namespace Nop.Web.Framework.Configuration
{
    public class WebOptimizerConfig : WebOptimizerOptions, IConfig
    {
		/// <summary>
        /// A value indicating whether JS file bundling and minification is enabled
        /// </summary>
        public bool EnableJsBundling { get; private set; } = true;

        /// <summary>
        /// A value indicating whether CSS file bundling and minification is enabled
        /// </summary>
        public bool EnableCssBundling { get; private set; } = true;

        /// <summary>
        /// Gets a section name to load configuration
        /// </summary>
        [JsonIgnore]
        public string Name => "WebOptimizer";
    }
}