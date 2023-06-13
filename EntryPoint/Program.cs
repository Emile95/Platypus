using Core;
using Core.ApplicationAction.Run;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Utils.Json;
using Web.Model;

ServerInstance serverInstance = new ServerInstance();
serverInstance.LoadConfiguration();
serverInstance.LoadApplications();

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

app.MapPost(@"/action", ([FromBody]ApplicationActionRunParameter runActionParameter) => 
{
    runActionParameter.ActionParameters = JsonHelper.GetDictObjectFromJsonElementsDict(runActionParameter.ActionParameters);
    return serverInstance.RunAction(runActionParameter);
});

app.MapPost(@"/action/cancel", ([FromBody] CancelRunningActionBody body) =>
{
    serverInstance.CancelRunningApplicationAction(body.Guid);
});

app.MapGet(@"/action/runnings", () =>
{
    return serverInstance.GetRunningApplicationActions();
});

app.MapPost(@"/application/install", ([FromBody] InstallApplicationBody body) =>
{
    serverInstance.InstallApplication(body.DllFilePath);
});

app.MapPut(@"/application/configuration", ([FromBody] Dictionary<string, object> body) =>
{
    
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();