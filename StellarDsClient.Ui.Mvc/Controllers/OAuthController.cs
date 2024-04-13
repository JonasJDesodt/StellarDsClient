using Microsoft.AspNetCore.Mvc;
using StellarDsClient.Sdk;
using StellarDsClient.Sdk.Abstractions;
using StellarDsClient.Sdk.Settings;
using StellarDsClient.Ui.Mvc.Attributes;
using StellarDsClient.Ui.Mvc.Providers;

namespace StellarDsClient.Ui.Mvc.Controllers
{
    [ProvideOAuthBaseAddress]
    public class OAuthController(OAuthApiService oAuthApiService, OAuthTokenProvider oAuthTokenProvider) : Controller
    {
        [HttpGet]
        public IActionResult Index([FromQuery] string returnUrl)
        {
            TempData["returnUrl"] = returnUrl;

            return View();
        }

        [HttpGet]
        public IActionResult SignIn([FromQuery] string returnUrl)
        {
            TempData["returnUrl"] = TempData["returnUrl"] ?? returnUrl ;

            var oAuthSettings = oAuthApiService.OAuthSettings;

            return Redirect($"{oAuthSettings.BaseAddress}/oauth?client_id={oAuthSettings.ClientId}&redirect_uri={oAuthSettings.RedirectUri}&response_type=code");
        }

        [HttpGet]
        public async Task<IActionResult> OAuthCallback([FromQuery] string code)
        {
            var tokens = await oAuthApiService.GetTokensAsync(code);

            await oAuthTokenProvider.BrowserSignIn(tokens);

            if (TempData["returnUrl"] is string returnUrl)
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> SignOut([FromQuery] string returnUrl)
        {
            await oAuthTokenProvider.BrowserSignOut();

            return LocalRedirect(returnUrl);
        }
    }
}