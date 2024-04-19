using StellarDsClient.Ui.Mvc.Extensions;

#if DEBUG
StellarDsClient.Ui.Mvc.Builders.DataStoreBuilder.CreateTables();
#endif

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.AddStellarDsClientServices();

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