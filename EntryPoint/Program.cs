using Core;
using Core.ApplicationAction.Run;
using EntryPoint.Model;
using Microsoft.AspNetCore.Mvc;
using Utils.Json;

ServerInstance serverInstance = new ServerInstance();
serverInstance.LoadConfiguration();
serverInstance.LoadApplications();
serverInstance.InitializeServerSocketHandlers();

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
    try
    {
        serverInstance.InstallApplication(body.DllFilePath);
    } catch (Exception ex)
    {
        return ex.Message;
    }
    return "application installed";
});

app.MapPost(@"/application/uninstall", ([FromBody] UninstallApplicationBody body) =>
{
    try
    {
        serverInstance.UninstalApplication(body.ApplicationGuid);
    } catch (Exception ex)
    {
        return ex.Message;
    }
    return "application uninstalled";
});

app.MapPost(@"/application/user", ([FromBody] UserCreationBody body) =>
{
    try
    {
        body.Data = JsonHelper.GetDictObjectFromJsonElementsDict(body.Data);
        serverInstance.AddUser(body.ConnectionMethodGUID, body.FullName, body.Email, body.Data);
    }
    catch (Exception ex)
    {
        return ex.Message;
    }

    return "user added";
});

app.MapPost(@"/application/user/connect", ([FromBody] UserConnection body) =>
{
    body.Credential = JsonHelper.GetDictObjectFromJsonElementsDict(body.Credential);
    try
    {
        return (object)serverInstance.UserConnect(body.Credential, body.ConnectionMethodGuid);
    } catch(Exception e)
    {
        return e.Message;
    }
    
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();