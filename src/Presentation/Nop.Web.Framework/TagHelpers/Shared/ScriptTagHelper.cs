using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Hosting;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.UI;

namespace Nop.Web.Framework.TagHelpers.Shared
{
    /// <summary>
    /// "script" tag helper
    /// </summary>
    [HtmlTargetElement(SCRIPT_TAG_NAME, Attributes = LOCATION_ATTRIBUTE_NAME)]
    [HtmlTargetElement(SCRIPT_TAG_NAME, Attributes = DEBUG_SRC_ATTRIBUTE_NAME)]
    public class ScriptTagHelper : TagHelper
    {
        #region Constants

        private const string DEBUG_SRC_ATTRIBUTE_NAME = "asp-debug-src";
        private const string LOCATION_ATTRIBUTE_NAME = "asp-location";
        private const string SCRIPT_TAG_NAME = "script";
        private const string SRC_ATTRIBUTE_NAME = "src";

        #endregion

        #region Fields

        private readonly IHtmlHelper _htmlHelper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        #endregion

        #region Ctor

        public ScriptTagHelper(IHtmlHelper htmlHelper,
            IWebHostEnvironment webHostEnvironment)
        {
            _htmlHelper = htmlHelper;
            _webHostEnvironment = webHostEnvironment;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously executes the tag helper with the given context and output
        /// </summary>
        /// <param name="context">Contains information associated with the current HTML tag</param>
        /// <param name="output">A stateful HTML element used to generate an HTML tag</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (output == null)
                throw new ArgumentNullException(nameof(output));

            //get JavaScript
            var childContent = await output.GetChildContentAsync();
            var script = childContent.GetContent();

            output.Attributes.RemoveAll(SRC_ATTRIBUTE_NAME);

            if (!string.IsNullOrEmpty(DebugSrc) && _webHostEnvironment.IsDevelopment())
                output.Attributes.SetAttribute(SRC_ATTRIBUTE_NAME, DebugSrc);
            else if (!string.IsNullOrEmpty(Src))
                output.Attributes.SetAttribute(SRC_ATTRIBUTE_NAME, Src.TrimStart('~'));

            var scriptTag = new TagBuilder(SCRIPT_TAG_NAME);

            if (!string.IsNullOrEmpty(script))
                scriptTag.InnerHtml.SetHtmlContent(new HtmlString(script));

            scriptTag.MergeAttributes(await output.GetAttributeDictionaryAsync());
            output.SuppressOutput();

            var tagHtml = await scriptTag.RenderHtmlContentAsync();

            if (Location == ResourceLocation.None)
                output.PostElement.AppendHtml(tagHtml + Environment.NewLine);
            else
                _htmlHelper.AddInlineScriptParts(Location, tagHtml);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Script path (e.g. full debug version). If empty, then minified version will be used
        /// </summary>
        [HtmlAttributeName(DEBUG_SRC_ATTRIBUTE_NAME)]
        public string DebugSrc { get; set; }

        /// <summary>
        /// Indicates where the script should be rendered
        /// </summary>
        [HtmlAttributeName(LOCATION_ATTRIBUTE_NAME)]
        public ResourceLocation Location { set; get; }

        /// <summary>
        /// Makes sure this taghelper runs after the built in WebOptimizer.
        /// </summary>
        public override int Order => 1;

        /// <summary>
        /// 
        /// </summary>
        [HtmlAttributeName(SRC_ATTRIBUTE_NAME)]
        public string Src { get; set; }

        #endregion
    }
}