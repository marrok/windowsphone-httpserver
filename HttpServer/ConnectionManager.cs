using HttpServer.Filters;
using HttpServer.Filters.Http;
using HttpServer.Filters.Security.Connection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.Sockets;

namespace HttpServer
{
    internal class ConnectionManager : IDisposable
    {
        private Semaphore _semaphore = new Semaphore(2, 2);
        private StreamSocketListener _listener;
        private string _port;
        private ConnectionFilter _connectionFilter;
        private HttpRequestResponseFilter _httpRequestResponseFilter;
        private HttpBaseAuthFilter _authFilter;
        private HttpRequestHandlerFilter _handlerFilter;
        private Filter _filterChain;

        internal ConnectionManager(string port)
        {
            _port = port;

            _handlerFilter = new HttpRequestHandlerFilter();
            _authFilter = new HttpBaseAuthFilter();

            _httpRequestResponseFilter = new HttpRequestResponseFilter();
            _httpRequestResponseFilter.AddHttpFilter(_handlerFilter);
            _httpRequestResponseFilter.AddHttpFilter(_authFilter);

            _connectionFilter = new ConnectionFilter();
            _connectionFilter.SetNext(_httpRequestResponseFilter);

            _filterChain = _connectionFilter;
        }

        internal void Start()
        {
            Task.Run(async () =>
            {
                _listener = new StreamSocketListener();
                _listener.ConnectionReceived += OnSocketListenerConnectionReceived;
                await _listener.BindServiceNameAsync(_port);
            });
        }

        internal void Stop()
        {
            _listener.Dispose();
        }

        private async void OnSocketListenerConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            using (args.Socket)
            {
                try
                {
                    _semaphore.WaitOne();
                    await _filterChain.DoFilter(args.Socket);
                }
                finally
                {
                    _semaphore.Release();
                }
            }
            GC.Collect();
        }

        internal void SetAuthCredidentials(String userName, String userPassword)
        {
            _authFilter.SetUserCredidentials(userName, userPassword);
        }

        internal void EnableAuthFilter(Boolean enable)
        {
            _authFilter.EnableBasicAuth(enable);
        }

        internal void EnableConnectionFilter(Boolean enable)
        {
            _connectionFilter.EnableFiltering(enable);
        }

        internal void RegisterConnectionFilter(AbstractConnectionFilter filter)
        {
            _connectionFilter.AddConnectionFilter(filter);
        }

        internal void RegisterHandler(HttpRequestHandler handler)
        {
            _handlerFilter.RegisterHandler(handler);
        }

        public void Dispose()
        {
            _semaphore.Dispose();
        }
    }
}
