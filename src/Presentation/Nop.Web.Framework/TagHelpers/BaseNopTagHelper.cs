using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.TagHelpers
{
    /// <summary>
    /// Represents base tag helper
    /// </summary>
    public class BaseNopTagHelper : TagHelper
    {
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

        #region Methods

        protected static async Task<string> GetAttributeValueAsync(TagHelperOutput output, string attributeName)
        {

            if (string.IsNullOrEmpty(attributeName) || !output.Attributes.TryGetAttribute(attributeName, out var attr))
                return null;

            if (attr.Value is string stringValue)
                return stringValue;
            
            return attr.Value switch
            {
                HtmlString htmlString => htmlString.ToString(),
                IHtmlContent content => await content.RenderHtmlContentAsync(),
                _ => default
            };
        }

        #endregion
    }
}