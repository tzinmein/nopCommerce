using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Get string value from tag helper output
        /// </summary>
        /// <param name="output">An information associated with the current HTML tag</param>
        /// <param name="attributeName">Name of the attribute</param>
        /// <returns>Matching name</returns>
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

        protected static async Task<IDictionary<string, string>> GetAttributeDictionaryAsync(TagHelperOutput output)
        {
            if (output is null)
                throw new ArgumentNullException(nameof(output));

            var result = new Dictionary<string, string>();

            if (output.Attributes.Count == 0)
                return result;

            foreach (var attrName in output.Attributes.Select(x => x.Name).Distinct())
            {
                result.Add(attrName, await GetAttributeValueAsync(output, attrName));
            }

            return result;
        }

        #endregion
    }
}