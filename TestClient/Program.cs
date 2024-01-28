using PlatypusAPI;
using PlatypusAPI.Exceptions;
using PlatypusAPI.User;

PlatypusServerConnection platypusServerConnection = new PlatypusServerConnection();

Dictionary<string, object> credentials = new Dictionary<string, object>();

credentials.Add("UserName", "admin");
credentials.Add("Password", "Pakipaki2");

platypusServerConnection.SetUserConnectionData(BuiltInUserConnectionMethodGuid.PlatypusUser, credentials);

try
{
    PlatypusServerApplication app = platypusServerConnection.Connect();
    var result = app.GetApplicationActionInfos();
    int x = 5;
}
catch (UserConnectionFailedException exception)
{
    Console.WriteLine(exception.Message);
}


Console.WriteLine("Hello, World!");