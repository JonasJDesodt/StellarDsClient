using Microsoft.AspNetCore.Mvc;
using StellarDsClient.Models.Mappers;
using StellarDsClient.Sdk;
using StellarDsClient.Sdk.Attributes;
using StellarDsClient.Sdk.Dto.Schema;
using StellarDsClient.Ui.Mvc.Extensions;
using StellarDsClient.Ui.Mvc.Providers;
using System.Reflection;

namespace StellarDsClient.Ui.Mvc.Controllers
{
    public class OAuthController(OAuthAccessTokenProvider oAuthAccessTokenProvider, SchemaApiService<OAuthAccessTokenProvider> schemaApiService) : Controller
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
            TempData["returnUrl"] = TempData["returnUrl"] ?? returnUrl;

            return Redirect($"{_oAuthApiService.OAuthBaseAddress}/oauth?client_id={_oAuthApiService.ClientId}&redirect_uri={_oAuthApiService.RedirectUri}&response_type=code");
        }

        [HttpGet]
        public async Task<IActionResult> OAuthCallback([FromQuery] string code)
        {
            var tokens = await _oAuthApiService.GetTokensAsync(code);

            await oAuthAccessTokenProvider.BrowserSignIn(tokens);

            await schemaApiService.SetTableSettings([typeof(List), typeof(ToDo)]);

            return TempData["returnUrl"] is not string returnUrl ? RedirectToAction("Index", "Home") : LocalRedirect(returnUrl);
        }

        [HttpGet]
        public async Task<IActionResult> SignOut([FromQuery] string returnUrl)
        {
            await oAuthAccessTokenProvider.BrowserSignOut();

            return LocalRedirect(returnUrl);
        }



    }
}