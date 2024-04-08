namespace StellarDsClient.Ui.Mvc.Models.Settings
{
    public class OAuthCookies
    {
        public required CookieOptions AccessToken { get; set; }

        public required CookieOptions RefreshToken { get; set; }
    }
}