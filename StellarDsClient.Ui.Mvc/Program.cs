using StellarDsClient.Ui.Mvc.Extensions;

#if DEBUG
using StellarDsClient.Builder.Library;
using StellarDsClient.Models.Mappers;
#endif

//todo: filters: show/hide buttons on desktop still work
//todo: check tier? (blobs) => or set option to run without blobs
//todo: check permissions
//todo: try/catch on dbBuilder + dispose the configurationbuilder? (when not in debug) or is it always the same instance?
//todo: chose dsBuilder or dbBuilder
//todo: filter between dates
//todo: db models in separate project
//todo: debug section in StellarDsSettings?
//todo: sign in the dsBuilder?

#if DEBUG
var stellarDsSettings = await DbBuilder.Run([typeof(List), typeof(ToDo)]);
Console.WriteLine($"Refresh the localhost page in the browser to go to the StellarDsClient web app.");
#else
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