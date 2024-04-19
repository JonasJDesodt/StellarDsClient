//using Microsoft.AspNetCore.Components;
//using Microsoft.AspNetCore.Mvc;
//using StellarDsClient.Builder.Api.Extensions;
//using StellarDsClient.Builder.Api.Helpers;
//using StellarDsClient.Sdk;
//using StellarDsClient.Sdk.Settings;

//var builder = WebApplication.CreateBuilder(args);

//var apiSettings = new ApiSettings
//{
//    BaseAddress = "https://api.stellards.io",
//    Name = "StellarDs",
//    Project = "fe14a12d-9099-47af-8490-08dc390dd619",
//    ReadOnlyToken =
//        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJhY2Nlc3MtdG9rZW4iLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllci10b2tlbiI6IjJkYTVlNmE0LWVjNzItNGZmNy04ZDJjLTA4ZGM1ODVmMDhjMCIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWVpZGVudGlmaWVyLXByb2plY3QiOiJmZTE0YTEyZC05MDk5LTQ3YWYtODQ5MC0wOGRjMzkwZGQ2MTkiLCJleHAiOjI1MzQwMjI5NzIwMCwiaXNzIjoiaHR0cHM6Ly9hcGkuc3RlbGxhcmRzLmlvIiwiYXVkIjoiaHR0cHM6Ly9hcGkuc3RlbGxhcmRzLmlvIn0.V6IHWBC0N5ca3TblObo_6IZZhw3fS4DlkTgzraRq68E",
//    Version = "v1"
//};
//var oAuthSettings = new OAuthSettings
//{
//    Name = "OAuth",
//    BaseAddress = "https://stellards.io",
//    ClientId = "68EC97E0-F3DD-470C-37F4-08DC5EE8D710",
//    ClientSecret = "d4c54c1818956c37155ecd504af860c5ca8ca139ec35aa8840227941fa66f08b",
//    RedirectUri = $"{EnvironmentHelpers.GetApplicationUrl()}/oauth/oauthcallback",
//};
//builder.AddServices(apiSettings, oAuthSettings);

//builder.Services.AddHttpClient("StellarDsClient", httpClient =>
//{
//    httpClient.BaseAddress = new Uri("https://localhost:7182");
//});

//var app = builder.Build();

////app.MapGet("/sign-in", (HttpContext httpContext) => Results.Redirect($"https://stellards.io/oauth?client_id={oAuthSettings.ClientId}&redirect_uri={oAuthSettings.RedirectUri}&response_type=code"));

////app.MapGet("/oauth/oauthcallback", async ([FromQuery] string code, HttpContext httpContext, OAuthApiService oAuthApiService, IHttpClientFactory httpClientFactory) =>
////{
////    //the state parameter is not implemented by StellarDs

////    var accessToken = await oAuthApiService.GetAccessToken(code);

////    return Results.Ok(new { AccessToken = accessToken });
////});

//app.MapPost("/stellar-datastore-settings", async ([FromBody] string accessToken) =>
//{

//    return Results.Ok(new
//    {
//        StellarDsSettings = new StellarDsCredentials
//        {
//            ApiCredentials = apiSettings,
//            OAuthCredentials = oAuthSettings,
//            TableSettings = new TableSettings { { "Tetten", 1 } }
//        }
//    });
//});

//app.Run();
