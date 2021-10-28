using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Web.Framework.UI;

namespace Nop.Web.Framework.TagHelpers.Shared
{
    /// <summary>
    /// "link" tag helper
    /// </summary>
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

        #region Methods

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (output == null)
                throw new ArgumentNullException(nameof(output));

            var href = await output.GetAttributeValueAsync("href");

            if (!string.IsNullOrEmpty(href))
                _htmlHelper.AddCssFileParts(href);

            //generate nothing
            output.SuppressOutput();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Makes sure this taghelper runs after the built in WebOptimizer.
        /// </summary>
        public override int Order => 12;

        #endregion
    }
}