using AspNetCore.Localization.Localization;
using AspNetCore.Localization.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AspNetCore.Localization.Controllers
{
    [CheckLanguageSeoCode]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly LanguageAccessor _languageAccessor;

        public HomeController(ILogger<HomeController> logger, 
            LanguageAccessor languageAccessor)
        {
            _logger = logger;
            _languageAccessor = languageAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ChangeLanguage([FromRoute]string langCode, string returnUrl = "")
        {
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");

            if (returnUrl.IsLocalizedUrl(Request.PathBase, true, out var _))
                returnUrl = returnUrl.RemoveLanguageSeoCodeFromUrl(Request.PathBase, true);

            returnUrl = returnUrl.AddLanguageSeoCodeToUrl(Request.PathBase, true, langCode);

            _languageAccessor.CurrentLanguage = langCode;

            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");

            return Redirect(returnUrl);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
