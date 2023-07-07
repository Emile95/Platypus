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
        private readonly Dictionary<string, UserAccountToken> _tokens;

        private string _userTokenRequestHeader = "user-token";
        public RestAPIHandler(ServerInstance serverInstance)
        {
            _serverInstance = serverInstance;
            _tokens = new Dictionary<string, UserAccountToken>();
        }
        public void Initialize(string[] args, int httpPort, double userTokenTimeout)
        {
            double userTokenTimeoutInMinutes = userTokenTimeout * 60000;

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

            MapPost<UserConnection>(app, @"/user/connect", false, (headers, userAccount, body) =>
            {
                UserAccount connectedUserAccount = _serverInstance.UserConnect(body.Credential, body.ConnectionMethodGuid);

                string newToken = GuidGenerator.GenerateFromEnumerable(_tokens.Keys);

                UserAccountToken userAccountToken = new UserAccountToken()
                {
                    UserAccount = connectedUserAccount,
                    Timer = new System.Timers.Timer()
                    {
                        Interval = userTokenTimeoutInMinutes,
                        AutoReset = false
                    }
                };

                _tokens.Add(newToken, userAccountToken);
                headers.Add(_userTokenRequestHeader, newToken);

                userAccountToken.Timer.Elapsed += (source, eventArgs) => CheckUserTokensOnTimedEvent(source, eventArgs, newToken);
                userAccountToken.Timer.Start();

                return "connection successfull";
            });

            MapPost<ApplicationActionRunParameter>(app, @"/action", true, (headers, userAccount, body) =>
            {
                return _serverInstance.RunAction(userAccount, body);
            });

            MapPost<CancelRunningActionBody>(app, @"/action/cancel", false, (headers, userAccount, body) =>
            {
                _serverInstance.CancelRunningApplicationAction(userAccount, body.Guid);
                return "run cancelled";
            });

            MapPost<InstallApplicationBody>(app, @"/application/install", false, (headers, userAccount, body) =>
            {
                _serverInstance.InstallApplication(userAccount, body.DllFilePath);
                return "application installed";
            });

            MapPost<UninstallApplicationBody>(app, @"/application/uninstall", false, (headers, userAccount, body) =>
            {
                _serverInstance.UninstalApplication(userAccount, body.ApplicationGuid);
                return "application uninstalled";
            });

            MapPost<UserCreationParameter>(app, @"/user", false, (headers, userAccount, body) =>
            {
                _serverInstance.AddUser(userAccount, body);
                return "user added";
            });

            MapGet(app, @"/action/runnings", (userAccount) =>
            {
                return _serverInstance.GetRunningApplicationActions(userAccount);
            });

            MapGet(app, @"/action", (userAccount) =>
            {
                return _serverInstance.GetApplicationActionInfos(userAccount);
            });

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
            app.MapPost(pattern, (requestDelegate) =>
            {
                return Task.Run(async () =>
                {
                    object responseObject = null;
                    try
                    {
                        UserAccount userAccount = null;
                        if (needUser)
                        {
                            string userToken = (string)requestDelegate.Request.Headers["user-token"];
                            if (_tokens.ContainsKey(userToken) == false) return;
                            userAccount = _tokens[userToken].UserAccount;
                        }

                        StreamReader reader = new StreamReader(requestDelegate.Request.Body);
                        string json = await reader.ReadToEndAsync();
                        BodyType body = JsonConvert.DeserializeObject<BodyType>(json);

                        responseObject = action(requestDelegate.Response.Headers, userAccount, body);

                    }
                    catch (Exception ex)
                    {
                        responseObject = ex.Message;
                    }

                    await requestDelegate.Response.WriteAsJsonAsync(responseObject);
                });
            });
        }

        private void MapGet(WebApplication app, string pattern, Func<UserAccount, object> action)
        {
            app.MapGet(pattern, (requestDelegate) =>
            {
                return Task.Run(async () =>
                {
                    object responseObject = null;
                    try
                    {
                        string userToken = (string)requestDelegate.Request.Headers["user-token"];
                        if (_tokens.ContainsKey(userToken) == false) return;
                        UserAccount userAccount = _tokens[userToken].UserAccount;

                        responseObject = action(userAccount);

                    }
                    catch (Exception ex)
                    {
                        responseObject = ex.Message;
                    }

                    await requestDelegate.Response.WriteAsJsonAsync(responseObject);
                });
            });
        }
        private void CheckUserTokensOnTimedEvent(Object source, System.Timers.ElapsedEventArgs e, string token)
        {
            _tokens.Remove(token);
        }
    }
}
