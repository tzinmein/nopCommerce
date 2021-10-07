using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Hosting;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.UI;

namespace Nop.Web.Framework.TagHelpers.Public
{
    /// <summary>
    /// "script" tag helper
    /// </summary>
    [HtmlTargetElement("script", Attributes = LOCATION_ATTRIBUTE_NAME)]
    [HtmlTargetElement("script", Attributes = DEBUG_SRC_ATTRIBUTE_NAME)]
    public class ScriptTagHelper : BaseNopTagHelper
    {
        #region Constants

        private const string LOCATION_ATTRIBUTE_NAME = "asp-location";
        private const string DEBUG_SRC_ATTRIBUTE_NAME = "asp-debug-src";

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

        #endregion

        #region Fields

        private readonly IHtmlHelper _htmlHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;

        #endregion

        #region Ctor

        public ScriptTagHelper(IHtmlHelper htmlHelper,
            IHttpContextAccessor httpContextAccessor,
            IWebHostEnvironment webHostEnvironment)
        {
            _htmlHelper = htmlHelper;
            _httpContextAccessor = httpContextAccessor;
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

            //contextualize IHtmlHelper
            var viewContextAware = _htmlHelper as IViewContextAware;
            viewContextAware?.Contextualize(ViewContext);

            //get JavaScript
            var childContent = await output.GetChildContentAsync();
            var script = childContent.GetContent();

            var src = await GetAttributeValueAsync(output, "src");

            if (!string.IsNullOrEmpty(src) && !string.IsNullOrEmpty(DebugSrc) && _webHostEnvironment.IsDevelopment())
            {
                output.Attributes.SetAttribute("src", DebugSrc);
            }

            var tagHtml = await _htmlHelper.BuildScriptTagAsync(await GetAttributeDictionaryAsync(output), script);

            output.SuppressOutput();

            if (Location == ResourceLocation.None)
                output.PostElement.AppendHtml(tagHtml + Environment.NewLine);
            else
                _htmlHelper.AddInlineScriptParts(Location, tagHtml);
        }

        #endregion
    }
}