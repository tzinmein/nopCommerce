using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Core.Configuration;
using Nop.Web.Framework.Configuration;
using WebOptimizer;
using WebOptimizer.Extensions;

namespace Nop.Web.Framework.TagHelpers.Shared
{
    /// <summary>
    /// CSS bundling tag helper
    /// </summary>
    [HtmlTargetElement(ASSET_TAG_NAME)]
    [HtmlTargetElement(BUNDLE_TAG_NAME)]
    public class BundleLinkTagHelper : TagHelper
    {
        #region Constants

        private const string ASSET_TAG_NAME = "link";
        private const string BUNDLE_TAG_NAME = "style-bundle";
        private const string BUNDLE_DESTINATION_KEY_NAME = "asp-bundle-dest-key";
        private const string BUNDLE_KEY_NAME = "asp-bundle-key";
        private const string EXCLUDE_FROM_BUNDLE_ATTRIBUTE_NAME = "asp-exclude-from-bundle";
        private const string HREF_ATTRIBUTE_NAME = "href";

        #endregion

        #region Fields

        private readonly AppSettings _appSettings;
        private readonly IAssetPipeline _assetPipeline;

        #endregion

        #region Ctor

        public BundleLinkTagHelper(AppSettings appSettings, IAssetPipeline assetPipeline)
        {
            _appSettings = appSettings;
            _assetPipeline = assetPipeline ?? throw new ArgumentNullException(nameof(assetPipeline));
        }

        #endregion

        #region Methods

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (output == null)
                throw new ArgumentNullException(nameof(output));

            output.TagName = ASSET_TAG_NAME;
            output.Attributes.SetAttribute("type", "text/css");
            output.Attributes.SetAttribute("rel", "stylesheet");
            output.TagMode = TagMode.SelfClosing;

            var bundleSuffix = _appSettings.Get<WebOptimizerConfig>().CssBundleSuffix;

            //to avoid collisions in controllers with the same names
            if (ViewContext.RouteData.Values.TryGetValue("area", out var area))
                bundleSuffix = $"{bundleSuffix}.{area}".ToLowerInvariant();

            if (string.Equals(context.TagName, BUNDLE_TAG_NAME))
                BundleDestinationKey ??= bundleSuffix;
            else
                BundleKey ??= bundleSuffix;

            if (!ExcludeFromBundle)
                output.HandleCssBundle(_assetPipeline, ViewContext, _appSettings.Get<WebOptimizerConfig>(), Href, BundleKey, BundleDestinationKey);
            else
                output.Attributes.SetAttribute("href", Href);

            return Task.CompletedTask;
        }

        #endregion

        #region Properties

        [HtmlAttributeName(EXCLUDE_FROM_BUNDLE_ATTRIBUTE_NAME)]
        public bool ExcludeFromBundle { get; set; }

        /// <summary>
        ///
        /// </summary>
        [HtmlAttributeName(BUNDLE_KEY_NAME)]
        public string BundleKey { get; set; }

        /// <summary>
        ///
        /// </summary>
        [HtmlAttributeName(BUNDLE_DESTINATION_KEY_NAME)]
        public string BundleDestinationKey { get; set; }

        [HtmlAttributeName(HREF_ATTRIBUTE_NAME)]
        public string Href { get; set; }

        /// <summary>
        /// The <see cref="ViewContext"/>.
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; } = default;

        #endregion
    }
}