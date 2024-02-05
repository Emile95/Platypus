using PlatypusContainer.Service;

namespace PlatypusContainer
{
    public class Container : IContainer
    {
        private readonly IHostedService _hostedService;

        public Container(IHostedService hostedService)
        {
            _hostedService = hostedService;
        }

        public void Run()
        {
            CancellationToken cancellationToken = new CancellationToken();
            _hostedService.RunAsync(cancellationToken);

            while (cancellationToken.IsCancellationRequested == false) 
                Thread.Sleep(200);
        }
    }
}
