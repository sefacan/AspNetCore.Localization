using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;

namespace AspNetCore.Localization.Localization
{
    public sealed class CheckLanguageSeoCodeAttribute : TypeFilterAttribute
    {
        #region Ctor

        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        public CheckLanguageSeoCodeAttribute() : base(typeof(CheckLanguageSeoCodeFilter))
        {
        }

        #endregion

        #region Nested filter

        /// <summary>
        /// Represents a filter that checks SEO friendly URLs for multiple languages and properly redirect if necessary
        /// </summary>
        private class CheckLanguageSeoCodeFilter : IActionFilter
        {
            private readonly LanguageAccessor _languageAccessor;

            public CheckLanguageSeoCodeFilter(LanguageAccessor languageAccessor)
            {
                _languageAccessor = languageAccessor;
            }

            /// <summary>
            /// Called before the action executes, after model binding is complete
            /// </summary>
            /// <param name="context">A context for action filters</param>
            public void OnActionExecuting(ActionExecutingContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                if (context.HttpContext.Request == null)
                    return;

                //only in GET requests
                if (!context.HttpContext.Request.Method.Equals(WebRequestMethods.Http.Get, StringComparison.InvariantCultureIgnoreCase))
                    return;

                //ensure that this route is registered and localizable (LocalizedRoute in RouteProvider)
                if (context.RouteData.Values["language"] == null)
                    return;

                //check whether current page URL is already localized URL
                var pageUrl = context.HttpContext.Request.Path.ToString();
                if (pageUrl.IsLocalizedUrl(context.HttpContext.Request.PathBase, true, out var _))
                    return;

                //not localized yet, so redirect to the page with working language SEO code
                pageUrl = pageUrl.AddLanguageSeoCodeToUrl(context.HttpContext.Request.PathBase, true, _languageAccessor.CurrentLanguage);
                context.Result = new LocalRedirectResult(pageUrl, false);
            }

            /// <summary>
            /// Called after the action executes, before the action result
            /// </summary>
            /// <param name="context">A context for action filters</param>
            public void OnActionExecuted(ActionExecutedContext context)
            {
            }
        }

        #endregion
    }
}