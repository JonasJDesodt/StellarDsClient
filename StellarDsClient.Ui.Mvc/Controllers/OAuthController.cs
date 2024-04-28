using Microsoft.AspNetCore.Mvc;
using StellarDsClient.Sdk;
using StellarDsClient.Sdk.Abstractions;
using StellarDsClient.Sdk.Settings;
using StellarDsClient.Ui.Mvc.Attributes;
using StellarDsClient.Ui.Mvc.Providers;

namespace StellarDsClient.Ui.Mvc.Controllers
{
    [ProvideOAuthBaseAddress]
    public class OAuthController(OAuthAccessTokenProvider oAuthAccessTokenProvider) : Controller
    {
        private readonly OAuthApiService _oAuthApiService = oAuthAccessTokenProvider.OAuthApiService;

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

            return Redirect($"{_oAuthApiService.OAuthBaseAddress}/oauth?client_id={_oAuthApiService.ClientId}&redirect_uri={_oAuthApiService.RedirectUri}&response_type=code");
        }

        [HttpGet]
        public async Task<IActionResult> OAuthCallback([FromQuery] string code)
        {
            var tokens = await _oAuthApiService.GetTokensAsync(code);

            await oAuthAccessTokenProvider.BrowserSignIn(tokens);

            if (TempData["returnUrl"] is string returnUrl)
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> SignOut([FromQuery] string returnUrl)
        {
            await oAuthAccessTokenProvider.BrowserSignOut();

            return LocalRedirect(returnUrl);
        }
    }
}