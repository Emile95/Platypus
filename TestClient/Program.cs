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

    /*Task[] tasks = new Task[4];
    for (int i = 0; i < 4; i++)
        tasks[i] = Task.Run(() =>
        {
            var result = app.GetApplicationActionInfos();
            Console.WriteLine(result.Count);
        });

    Task.WaitAll(tasks);*/

    var result = app.GetApplicationActionInfos();

    Console.WriteLine(result.Count);

    int x = 5;
}
catch (UserConnectionFailedException exception)
{
    Console.WriteLine(exception.Message);
}

Console.WriteLine("Hello, World!");