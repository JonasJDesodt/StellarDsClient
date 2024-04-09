using Microsoft.AspNetCore.Http;
using StellarDsClient.Generator.Models;
using StellarDsClient.Sdk.Settings;
using StellarDsClient.Ui.Mvc.Models.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.IdentityModel.JsonWebTokens;

namespace StellarDsClient.Generator.Helpers
{
    internal static class FileHelpers
    {
        public static void WriteAppSettings(int listTableId, int taskTableId, int port, Guid clientId, string clientSecret, Guid projectId, string readOnlyAccessToken)
        {
            var appSettings = new AppSettings
            {
                ApiSettings = new ApiSettings
                {
                    BaseAddress = "https://api.stellards.io",
                    Name = "StellarDs",
                    Project = projectId.ToString(),
                    ReadOnlyToken = readOnlyAccessToken,
                    Version = "v1"
                },
                OAuthSettings = new OAuthSettings
                {
                    BaseAddress = "https://stellards.io",
                    ClientId = clientId.ToString(),
                    ClientSecret = clientSecret,
                    Name = "OAuth",
                    RedirectUri = $"https://localhost:{port}/oauth/oauthcallback",
                },
                TableSettings = new TableSettings
                {
                    ListTableId = listTableId,
                    TaskTableId = taskTableId
                },
                CookieSettings = new CookieSettings
                {
                    OAuthCookies = new OAuthCookies
                    {
                        AccessToken = new CookieOptions
                        {
                            IsEssential = true,
                            HttpOnly = true,
                            Secure = true
                        },
                        RefreshToken = new CookieOptions
                        {
                            IsEssential = true,
                            HttpOnly = true,
                            Secure = true
                        }
                    },
                },
                Logging = new Logging
                {
                    LogLevel = new LogLevel
                    {
                        Default = "Information",
                        MicrosoftAspNetCore = "Warning"
                    }
                },
                AllowedHosts = "*"
            };

            var jsonString = JsonSerializer.Serialize(appSettings, new JsonSerializerOptions { WriteIndented = true });

            var folderPath = Path.Combine(AppContext.BaseDirectory, "GeneratedFiles");
            
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            File.WriteAllText(Path.Combine(folderPath, "appsettings.json"), jsonString);

            
       
            //string relativePath = "../StellarDsClient.Ui.Mvc"; // Adjust based on your actual relative path
            //string targetProjectRoot = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), relativePath));
            //string targetFilePath = Path.Combine(targetProjectRoot, "appsettings.json");
            //File.WriteAllText(targetFilePath, "test");

        

            Console.WriteLine($"appsettings.json file path: {AppContext.BaseDirectory + "appsettings.json"}");
        }
    }
}