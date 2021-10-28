using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Core.Configuration;
using Nop.Web.Framework.Configuration;

namespace Nop.Web.Framework.TagHelpers.Shared
{
    /// <summary>
    /// CSS bundling tag helper
    /// </summary>
    [HtmlTargetElement(ASSET_TAG_NAME)]
    [HtmlTargetElement(BUNDLE_TAG_NAME)]
    public class BundleLinkTagHelper : WebOptimizer.TagHelpersDynamic.LinkTagHelper
    {
        #region Constants

        private const string ASSET_TAG_NAME = "link";
        private const string BUNDLE_TAG_NAME = "style-bundle";

        #endregion

        #region Fields

        private readonly AppSettings _appSettings;

        #endregion

        #region Ctor

        public BundleLinkTagHelper(
            AppSettings appSettings,
            IWebHostEnvironment hostingEnvironment,
            TagHelperMemoryCacheProvider cache,
            IFileVersionProvider fileVersionProvider,
            HtmlEncoder htmlEncoder,
            JavaScriptEncoder javaScriptEncoder,
            IUrlHelperFactory urlHelperFactory,
            IServiceProvider serviceProvider) : base(hostingEnvironment, cache, fileVersionProvider, htmlEncoder, javaScriptEncoder, urlHelperFactory, serviceProvider)
        {
            _appSettings = appSettings;
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

            if (!_appSettings.Get<WebOptimizerConfig>().EnableCssBundling)
                return Task.CompletedTask;

            var bundleKey = _appSettings.Get<WebOptimizerConfig>().CssBundleSuffix;

            if (System.Globalization.CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft)
                bundleKey = $"{bundleKey}.rtl".ToLowerInvariant();

            //to avoid collisions in controllers with the same names
            if (ViewContext.RouteData.Values.TryGetValue("area", out var area))
                bundleKey = $"{bundleKey}.{area}".ToLowerInvariant();

            if (!string.IsNullOrEmpty(Href) && string.IsNullOrEmpty(BundleKey))
                BundleKey = bundleKey;

            if (string.Equals(context.TagName, BUNDLE_TAG_NAME) && string.IsNullOrEmpty(BundleDestinationKey))
                BundleDestinationKey = bundleKey;

            return base.ProcessAsync(context, output);
        }

        #endregion
    }
}