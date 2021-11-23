using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Nop.Core.Domain.Security;

namespace Nop.Web.Framework.Mvc.Routing
{
    /// <summary>
    /// Represents custom overridden redirect result executor
    /// </summary>
    public class NopRedirectResultExecutor : RedirectResultExecutor
    {
        #region Fields

        private readonly SecuritySettings _securitySettings;

        #endregion

        #region Ctor

        public NopRedirectResultExecutor(ILoggerFactory loggerFactory, 
            IUrlHelperFactory urlHelperFactory,
            SecuritySettings securitySettings) : base(loggerFactory, urlHelperFactory)
        {
            _securitySettings = securitySettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Execute passed redirect result
        /// </summary>
        /// <param name="context">Action context</param>
        /// <param name="result">Redirect result</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override Task ExecuteAsync(ActionContext context, RedirectResult result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            if (_securitySettings.AllowNonAsciiCharactersInHeaders)
            {
                //passed redirect URL may contain non-ASCII characters, that are not allowed now (see https://github.com/aspnet/KestrelHttpServer/issues/1144)
                //so we force to encode this URL before processing
                var url = WebUtility.UrlDecode(result.Url);
                var urlSplited = url.Split("?");
                var urlEscaped = string.Join("/", urlSplited[0].Split("/").Select(s => Uri.EscapeDataString(s)));

                result.Url = urlSplited.Length > 1 ? $"{urlEscaped}?{EscapeQueryString(urlSplited[1])}" : urlEscaped;
            }

            return base.ExecuteAsync(context, result);
        }
        private static string EscapeQueryString(string queryString)
        {
            var query = queryString.Split('&', '=');
            for (var i = 0; i < query.Length; i++)
            {
                //odd elements are all values to be encoded
                if (i % 2 == 1)
                {
                    var data = Uri.EscapeDataString(query[i]);
                    queryString = queryString.Replace(query[i], data);
                }
            }
            return queryString;
        }

        #endregion
    }
}