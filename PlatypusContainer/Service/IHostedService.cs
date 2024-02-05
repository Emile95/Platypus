namespace PlatypusContainer.Service
{
    public interface IHostedService
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}
