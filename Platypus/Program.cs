using Application;
using Application.Action;
using Microsoft.AspNetCore.Mvc;
using Utils.Json;

ApplicationInstance applicationInstance = new ApplicationInstance();
applicationInstance.LoadConfiguration();
int nbApplications = applicationInstance.LoadApplications();
Console.WriteLine($"{nbApplications} applications loaded");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapPost(@"/action", ([FromBody]RunActionParameter runActionParameter) => 
{
    runActionParameter.ActionParameters = JsonHelper.GetDictObjectFromJsonElementsDict(runActionParameter.ActionParameters);
    return applicationInstance.RunAction(runActionParameter);
});

app.MapPost(@"/action/cancel", (string guid) =>
{
    applicationInstance.CancelRunningApplicationAction(guid);
});

app.MapGet(@"/action/runnings", () =>
{
    return applicationInstance.GetRunningApplicationActions();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();