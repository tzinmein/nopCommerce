using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Settings
{
    public partial record WebOptimizerConfigModel : BaseNopModel, IConfigModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Configuration.AppSettings.WebOptimizer.EnableJsBundling")]
        public bool EnableJsBundling { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.WebOptimizer.EnableCssBundling")]
        public bool EnableCssBundling { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.WebOptimizer.EnableDiskCache")]
        public bool? EnableDiskCache { get; set; }

        [NopResourceDisplayName("Admin.Configuration.AppSettings.WebOptimizer.CacheDirectory")]
        public string CacheDirectory { get; set; }

        #endregion
    }
}
