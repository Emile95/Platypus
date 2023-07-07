using Core.RestAPI.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using PlatypusAPI.ApplicationAction.Run;
using PlatypusAPI.User;
using Utils.GuidGeneratorHelper;

namespace Core.RestAPI
{
    public class RestAPIHandler
    {
        private readonly ServerInstance _serverInstance;
        private readonly Dictionary<string, UserAccount> _tokens;

        private string _userTokenRequestHeader = "user-token";
        public RestAPIHandler(ServerInstance serverInstance)
        {
            _serverInstance = serverInstance;
            _tokens = new Dictionary<string, UserAccount>();
        }
        public void Initialize(string[] args, int httpPort)
        {
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

            MapPost<ApplicationActionRunParameter>(app, @"/action", true, (headers, userAccount, body) => {
                return _serverInstance.RunAction(userAccount, body);
            });

            MapPost<UserConnection>(app, @"/user/connect", false, (headers, userAccount, body) => {
                try
                {
                    UserAccount connectedUserAccount = _serverInstance.UserConnect(body.Credential, body.ConnectionMethodGuid);
                    string newToken = GuidGenerator.GenerateFromEnumerable(_tokens.Keys);
                    _tokens.Add(newToken, connectedUserAccount);
                    headers.Add(_userTokenRequestHeader, newToken);
                    return "orion";
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            });

            /*
            app.MapPost(@"/action", ([FromBody] ApplicationActionRunParameter runActionParameter) =>
            {
                runActionParameter.ActionParameters = JsonHelper.GetDictObjectFromJsonElementsDict(runActionParameter.ActionParameters);
                return _serverInstance.RunAction(runActionParameter);
            });

            app.MapPost(@"/action/cancel", ([FromBody] CancelRunningActionBody body) =>
            {
                _serverInstance.CancelRunningApplicationAction(body.Guid);
            });

            app.MapGet(@"/action/runnings", () =>
            {
                return _serverInstance.GetRunningApplicationActions();
            });

            app.MapPost(@"/application/install", ([FromBody] InstallApplicationBody body) =>
            {
                try
                {
                    _serverInstance.InstallApplication(body.DllFilePath);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
                return "application installed";
            });

            app.MapPost(@"/application/uninstall", ([FromBody] UninstallApplicationBody body) =>
            {
                try
                {
                    _serverInstance.UninstalApplication(body.ApplicationGuid);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
                return "application uninstalled";
            });

            app.MapPost(@"/application/user", ([FromBody] UserCreationParameter body) =>
            {
                try
                {
                    body.Data = JsonHelper.GetDictObjectFromJsonElementsDict(body.Data);
                    _serverInstance.AddUser(body);
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
                    UserAccount userAccount = _serverInstance.UserConnect(body.Credential, body.ConnectionMethodGuid);
                    string newToken = GuidGenerator.GenerateFromEnumerable(_tokens.Keys);
                    _tokens.Add(newToken, userAccount);
                    return newToken;
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            });
            */

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run($"https://localhost:{httpPort}");
        }

        private void MapPost<BodyType>(WebApplication app, string pattern, bool needUser, Func<IHeaderDictionary, UserAccount, BodyType, object> action)
            where BodyType : class
        {
            app.MapPost(pattern, (requestDelegate) => {
                return Task.Run(async () => {
                    UserAccount userAccount = null;
                    if(needUser)
                    {
                        string userToken = (string)requestDelegate.Request.Headers["user-token"];
                        if (_tokens.ContainsKey(userToken) == false) return;
                        userAccount = _tokens[userToken];
                    }

                    StreamReader reader = new StreamReader(requestDelegate.Request.Body);
                    string json = await reader.ReadToEndAsync();
                    BodyType body = JsonConvert.DeserializeObject<BodyType>(json);

                    object obj = action(requestDelegate.Response.Headers, userAccount, body);
                    await requestDelegate.Response.WriteAsJsonAsync(obj);
                });
            });
        }
    }
}
