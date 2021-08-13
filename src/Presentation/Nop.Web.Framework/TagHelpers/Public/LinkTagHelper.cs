using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.UI;

namespace Nop.Web.Framework.TagHelpers.Public
{
    [HtmlTargetElement("link")]
    public class LinkTagHelper : TagHelper
    {
        #region Fields

        private readonly IHtmlHelper _htmlHelper;

        #endregion

        #region Ctor

        public LinkTagHelper(IHtmlHelper htmlHelper)
        {
            _htmlHelper = htmlHelper;
        }

        #endregion

        #region Utils

        protected static async Task<string> GetHrefValueAsync(TagHelperOutput output)
        {
            if (!output.Attributes.TryGetAttribute("href", out var attr))
            {
                return null;
            }

            if (attr.Value is string stringValue)
            {
                return stringValue;
            }
            
            return attr.Value switch
            {
                HtmlString htmlString => htmlString.ToString(),
                IHtmlContent content => await content.RenderHtmlContentAsync(),
                _ => default
            };
        }

        #endregion

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (output == null)
                throw new ArgumentNullException(nameof(output));

            //contextualize IHtmlHelper
            var viewContextAware = _htmlHelper as IViewContextAware;
            viewContextAware?.Contextualize(ViewContext);

            var href = await GetHrefValueAsync(output);

            if (href is not null)
            {
                _htmlHelper.AddCssFileParts(href);

                //generate nothing
                output.SuppressOutput();
            }
        }

        #region Properties

        /// <summary>
        /// Makes sure this taghelper runs after the built in WebOptimizer.
        /// </summary>
        public override int Order => 12;

        /// <summary>
        /// ViewContext
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        #endregion
    }
}