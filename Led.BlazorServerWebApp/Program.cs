using Led.BlazorServerWebApp;
using Led.Library.Matrices;
using Led.Library.Settings;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);

#region Configure services

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddHttpClient(); // For REST API calls

//Services
ApplicationSettings settings = ApplicationSettings.Load();
builder.Services.AddSingleton<Hub75Matrix>(new Hub75Matrix(64, 64, settings.Hub75Matrix.RenderDelay, settings.Hub75Matrix.PwmDuration));

#endregion

var app = builder.Build();

#region Configure

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.MapControllers(); // For REST API controllers
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllers();
//});
app.Lifetime.ApplicationStopping.Register(() => { app.Services.GetService<Hub75Matrix>()?.Dispose(); });

#endregion


app.Run();