using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Localization.Localization
{
    public class RouteSegmentCultureProvider : IRequestCultureProvider
    {
        public Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            if (httpContext.Request == null)
                return Task.FromResult<ProviderCultureResult>(null);

            //only in GET requests
            if (!httpContext.Request.Method.Equals(HttpMethods.Get, StringComparison.InvariantCultureIgnoreCase))
                return Task.FromResult<ProviderCultureResult>(null);

            //ensure that this route is registered and localizable
            if (httpContext.Request.RouteValues["language"] == null)
                return Task.FromResult<ProviderCultureResult>(null);

            //check whether current page URL is already localized URL
            var pageUrl = httpContext.Request.Path.ToString();
            if (pageUrl.IsLocalizedUrl(httpContext.Request.PathBase, true, out var culture))
            {
                using (var scope = httpContext.RequestServices.CreateScope())
                {
                    var languages = scope.ServiceProvider.GetService<AppDataContext>().Languages.ToList();
                    var language = languages.FirstOrDefault(l => l.TwoLetterIsoCode == culture);
                    if (language == null)
                        return Task.FromResult<ProviderCultureResult>(null);

                    return Task.FromResult(new ProviderCultureResult(language.Culture));
                }
            }

            return Task.FromResult<ProviderCultureResult>(null);
        }
    }
}