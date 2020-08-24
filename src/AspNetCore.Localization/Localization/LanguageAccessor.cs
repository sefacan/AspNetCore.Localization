using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System.Linq;

namespace AspNetCore.Localization.Localization
{
    public class LanguageAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDataContext _dataContext;

        private string _cachedLanguage;

        public LanguageAccessor(IHttpContextAccessor httpContextAccessor, 
            AppDataContext dataContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _dataContext = dataContext;
        }

        protected string GetLanguageFromUrl()
        {
            if (_httpContextAccessor.HttpContext?.Request == null)
                return null;

            var path = _httpContextAccessor.HttpContext.Request.Path.Value;
            if (!path.IsLocalizedUrl(_httpContextAccessor.HttpContext.Request.PathBase, false, out var uniqueSeoCode))
                return null;

            return uniqueSeoCode;
        }

        protected string GetLanguageFromRequest()
        {
            if (_httpContextAccessor.HttpContext?.Request == null)
                return null;

            var requestCulture = _httpContextAccessor.HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture;
            if (requestCulture == null)
                return null;

            var uniqueSeoCode = requestCulture.Culture.TwoLetterISOLanguageName;
            return uniqueSeoCode;
        }

        public string CurrentLanguage
        {
            get
            {
                if (_cachedLanguage != null)
                    return _cachedLanguage;

                var detectedLanguage = GetLanguageFromUrl();

                if (detectedLanguage == null)
                    detectedLanguage = GetLanguageFromRequest();

                var languages = _dataContext.Languages.ToList();
                if (detectedLanguage == null)
                    detectedLanguage = languages.FirstOrDefault()?.TwoLetterIsoCode;

                //cache the found language
                _cachedLanguage = detectedLanguage;

                return _cachedLanguage;
            }
            set
            {
                //then reset the cached value
                _cachedLanguage = null;
            }
        }
    }
}