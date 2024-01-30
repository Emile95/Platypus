using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using PlatypusAPI.ApplicationAction.Run;
using PlatypusAPI.ServerFunctionParameter;
using PlatypusAPI.User;
using PlatypusUtils;

namespace Core.Network.RestAPI
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

            app.MapPost(@"/user/connect", CreateRequestDelegate<UserConnectionParameter>(false, (headers, userAccount, body) =>
            {
                UserAccount connectedUserAccount = _serverInstance.UserConnect(body);

                string newToken = Utils.GenerateGuidFromEnumerable(_tokens.Keys);

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
            }));

            app.MapPost(@"/action", CreateRequestDelegate<ApplicationActionRunParameter>(true, (headers, userAccount, body) =>
            {
                return _serverInstance.RunAction(userAccount, body);
            }));


            app.MapPost(@"/action/cancel", CreateRequestDelegate<CancelRunningActionParameter>(true, (headers, userAccount, body) =>
            {
                _serverInstance.CancelRunningApplicationAction(userAccount, body);
                return "run cancelled";
            }));

            app.MapPost(@"/application/install", CreateRequestDelegate<InstallApplicationParameter>(true, (headers, userAccount, body) =>
            {
                if(_serverInstance.InstallApplication(userAccount, body))
                    return "application installed";
                return "application failed to install";
            }));

            app.MapPost(@"/application/uninstall", CreateRequestDelegate<UninstallApplicationParameter>(true, (headers, userAccount, body) =>
            {
                _serverInstance.UninstalApplication(userAccount, body);
                return "application uninstalled";
            }));

            app.MapPost(@"/user", CreateRequestDelegate<UserCreationParameter>(true, (headers, userAccount, body) =>
            {
                _serverInstance.AddUser(userAccount, body);
                return "user added";
            }));

            app.MapPut(@"/user", CreateRequestDelegate<UserUpdateParameter>(true, (headers, userAccount, body) =>
            {
                _serverInstance.UpdateUser(userAccount, body);
                return "user updated";
            }));

            app.MapDelete(@"/user", CreateRequestDelegate<RemoveUserParameter>(true, (headers, userAccount, body) =>
            {
                _serverInstance.RemoveUser(userAccount, body);
                return "user removed";
            }));

            app.MapGet(@"/action/runnings", CreateRequestDelegate<object>(true, (headers, userAccount, body) =>
            {
                return _serverInstance.GetRunningApplicationActions(userAccount);
            }));

            app.MapGet(@"/action", CreateRequestDelegate<object>(true, (headers, userAccount, body) =>
            {
                return _serverInstance.GetApplicationActionInfos(userAccount);
            }));

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run($"https://localhost:{httpPort}");
        }

        private RequestDelegate CreateRequestDelegate<BodyType>(bool needUser, Func<IHeaderDictionary, UserAccount, BodyType, object> action)
            where BodyType : class
        {
            return (requestDelegate) =>
            {
                return Task.Run(async () =>
                {
                    object responseObject = null;
                    try
                    {
                        UserAccount userAccount = null;
                        if (needUser)
                        {
                            string userToken = (string)requestDelegate.Request.Headers[_userTokenRequestHeader];

                            if (userToken == null) throw new Exception($"need '{_userTokenRequestHeader}' in the request header");
                            if (_tokens.ContainsKey(userToken) == false) throw new Exception($"invalid '{_userTokenRequestHeader}' in the request header");

                            UserAccountToken userAccountToken = _tokens[userToken];
                            userAccount = userAccountToken.UserAccount;
                            userAccountToken.Timer.Stop();
                            userAccountToken.Timer.Start();
                        }

                        BodyType body = null;
                        if (requestDelegate.Request.Body != null)
                        {
                            StreamReader reader = new StreamReader(requestDelegate.Request.Body);
                            string json = await reader.ReadToEndAsync();
                            body = JsonConvert.DeserializeObject<BodyType>(json);
                        }

                        responseObject = action(requestDelegate.Response.Headers, userAccount, body);

                    }
                    catch (Exception ex)
                    {
                        responseObject = ex.Message;
                    }

                    await requestDelegate.Response.WriteAsJsonAsync(responseObject);
                });
            };
        }

        private void CheckUserTokensOnTimedEvent(object source, System.Timers.ElapsedEventArgs e, string token)
        {
            _tokens.Remove(token);
        }
    }
}
