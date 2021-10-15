using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Nop.Core.Configuration;
using Nop.Web.Framework.Configuration;
using Nop.Web.Framework.UI;

namespace Nop.Web.Framework.TagHelpers.Shared
{
    [HtmlTargetElement(ASSET_TAG_NAME, Attributes = EXCLUDE_FROM_BUNDLE_ATTRIBUTE_NAME)]
    [HtmlTargetElement(ASSET_TAG_NAME)]
    [HtmlTargetElement(BUNDLE_TAG_NAME)]
    public class BundleScriptTagHelper : WebOptimizer.TagHelpersDynamic.ScriptTagHelper
    {
        #region Constants

        private const string ASSET_TAG_NAME = "script";
        private const string BUNDLE_TAG_NAME = "script-bundle";
        private const string EXCLUDE_FROM_BUNDLE_ATTRIBUTE_NAME = "exclude-from-bundle";

        #endregion

        #region Fields

        private readonly AppSettings _appSettings;

        #endregion

        #region Ctor

        public BundleScriptTagHelper(
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

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (output == null)
                throw new ArgumentNullException(nameof(output));

            if (!output.Attributes.ContainsName("type")) // skip other types e.g. text/template
                output.Attributes.SetAttribute("type", "text/javascript");

            output.TagName = ASSET_TAG_NAME;
            output.TagMode = TagMode.StartTagAndEndTag;

            if (!_appSettings.Get<WebOptimizerConfig>().EnableJsBundling)
            {
                return;
            }
            else if (ExcludeFromBundle == true)
            {
                output.Attributes.SetAttribute("src", Src?.TrimStart('~'));
                return;
            }

            var bundleKey = _appSettings.Get<WebOptimizerConfig>().JavaScriptBundleSuffix;

            //to avoid collisions in controllers with the same names
            if (ViewContext.RouteData.Values.TryGetValue("area", out var area))
                bundleKey = $"{bundleKey}.{area}".ToLowerInvariant();

            if (!string.IsNullOrEmpty(Src) && string.IsNullOrEmpty(BundleKey))
                BundleKey = bundleKey;

            if (string.Equals(context.TagName, BUNDLE_TAG_NAME) && string.IsNullOrEmpty(BundleDestinationKey))
                BundleDestinationKey = bundleKey;

            await base.ProcessAsync(context, output);
        }

        #endregion

        #region Properties

        [HtmlAttributeName(EXCLUDE_FROM_BUNDLE_ATTRIBUTE_NAME)]
        public bool? ExcludeFromBundle { get; set; }

        #endregion

    }
}