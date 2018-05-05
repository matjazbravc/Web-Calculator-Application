using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace WebCalculatorService
{
    public class OwinCommunicationListener : ICommunicationListener
    {
        private readonly string _appRoot;
        private readonly ServiceContext _serviceContext;
        private readonly IOwinAppBuilder _startup;
        private string _listeningAddress;
        private IDisposable _serverHandle;

        public OwinCommunicationListener(IOwinAppBuilder startup, ServiceContext serviceContext, string appRoot)
        {
            _startup = startup;
            _serviceContext = serviceContext;
            _appRoot = appRoot;
        }

        public void Abort()
        {
            StopWebServer();
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            StopWebServer();
            return Task.FromResult(true);
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            var serviceEndpoint = _serviceContext.CodePackageActivationContext.GetEndpoint("ServiceEndpoint");
            var port = serviceEndpoint.Port;
            _listeningAddress = $"http://+:{port}/{_appRoot}/";

            _serverHandle = WebApp.Start(_listeningAddress,
                appBuilder => _startup.Configuration(appBuilder));
            var resultAddress = _listeningAddress.Replace("+",
                FabricRuntime.GetNodeContext().IPAddressOrFQDN);
            ServiceEventSource.Current.Message("Listening on {0}", resultAddress);
            return Task.FromResult(resultAddress);
        }

        private void StopWebServer()
        {
            if (_serverHandle == null)
            {
                return;
            }
            try
            {
                _serverHandle.Dispose();
            }
            catch (ObjectDisposedException)
            {
                StopWebServer();
                throw;
            }
        }
    }
}