using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.JsonWebTokens;
using StellarDsClient.Sdk;
using StellarDsClient.Ui.Mvc.Models.Settings;
using StellarDsClient.Ui.Mvc.Providers;
using StellarDsClient.Ui.Mvc.Stores;
using System.Diagnostics;
using StellarDsClient.Builder.Library.Models;
using StellarDsClient.Ui.Mvc.Extensions;

#if DEBUG
using StellarDsClient.Builder.Library;
using StellarDsClient.Dto.Data.Result;
#endif

//todo: filters: show/hide buttons on desktop still work
//todo: check tier? (blobs)
//todo: check permissions


#if DEBUG
var stellarDsSettings = await new DbBuilder().Run([typeof(List), typeof(ToDo)]);
Console.WriteLine($"Refresh the localhost page in the browser to go to the StellarDsclient web app.");
#else
//todo: dispose the configurationbuilder? or is it always the same instance?
var stellarDsSettings = new ConfigurationBuilder().AddJsonFile("appsettings.StellarDs.json", false).Build().Get<StellarDsSettings>() ?? throw new NullReferenceException("Unable to get the StellarDsSettings");
#endif

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.AddStellarDsClientServices(stellarDsSettings);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "toDo",
    pattern: "{controller=ToDo}/{listId}/{action=Index}/{id?}");

app.Run();