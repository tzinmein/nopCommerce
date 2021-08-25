using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Web.Framework.UI;

namespace Nop.Web.Framework.TagHelpers.Public
{
    [HtmlTargetElement("link")]
    public class LinkTagHelper : BaseNopTagHelper
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

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (output == null)
                throw new ArgumentNullException(nameof(output));

            //contextualize IHtmlHelper
            var viewContextAware = _htmlHelper as IViewContextAware;
            viewContextAware?.Contextualize(ViewContext);

            var href = await GetAttributeValueAsync(output, "href");

            if (href is not null)
            {
                _htmlHelper.AddCssFileParts(href);

                //generate nothing
                output.SuppressOutput();
            }
        }
    }
}