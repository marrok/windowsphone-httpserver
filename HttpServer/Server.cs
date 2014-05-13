using HttpServer.Filters.Security.Connection;
using HttpServer.Logger;
using HttpServer.Messages.Request;
using HttpServer.StaticResource;
using HttpServer.Streaming;
using System;

namespace HttpServer
{
    public class Server : IDisposable
    {
        private ConnectionManager _manager;
        private SimpleLogger _logger;
        private StreamingSocket _socket;

        public Server()
            : this("80", LogLevel.INFO, null, 0)
        {
        }

        public Server(LogLevel logLevel, LoggerDelegate loggerDelegate, int maxLogBufferSize)
            : this("80", logLevel, loggerDelegate, maxLogBufferSize)
        {
        }

        public Server(string port, LogLevel logLevel, LoggerDelegate loggerDelegate, int maxLogBufferSize)
        {
            _logger = SimpleLogger.InitializeLogger(logLevel, loggerDelegate, maxLogBufferSize);
            _manager = new ConnectionManager(port);
            _socket = new StreamingSocket();
        }

        public void Start()
        {
            _logger.Info("Starting server");
            _manager.Start();
            _logger.Info("Server started");
        }

        public void Stop()
        {
            _logger.Info("Stopping server");
            _manager.Start();
            _logger.Info("Server stopped");
        }

        public void ChangeLoggerLevel(LogLevel logLevel)
        {
            SimpleLogger.GetLogger().ChangeLogLevel(logLevel);
        }

        public void StartStreaming(String destinationIp, String destinationPort, String pingPort, StreamingSocketRunDelegate runDelegate, StreamingSocketStopDelegate stopDelegate)
        {
            _socket.Start(destinationIp, destinationPort, pingPort, runDelegate, stopDelegate);
        }

        public void StopStreaming()
        {
            _socket.Stop();
        }

        public void RegisterStaticResource(string resourcePath, string resourceUri, StaticResourceType type)
        {
            var handler = StaticResourceManager.CreateStaticResourceHandler(resourcePath, resourceUri, type);
            _manager.RegisterHandler(handler);
        }

        public void RegisterHandler(HttpRequestType requestType, String uri, Boolean isRegexp, HttpRequestHandlerDelegate handlerDelegate)
        {
            var handler = new HttpRequestHandler
            { HandledUri = uri,
                HandleUriWithRegexp = isRegexp,
                SupportedRequestType = requestType,
                HandlerDelegate = handlerDelegate };
            handler.BuildHandler();
            _manager.RegisterHandler(handler);
        }

        public void EnableConnectionFilter(Boolean enable)
        {
            _manager.EnableConnectionFilter(enable);
        }

        public void AddRemoteBannedIpEqualsConnectionFilter(String bannedIp)
        {
            _manager.RegisterConnectionFilter(new RemoteBannedIpEqualsConnectionFilter(bannedIp));
        }

        public void AddLocalBannedIpEqualsConnectionFilter(String bannedIp)
        {
            _manager.RegisterConnectionFilter(new LocalBannedIpEqualsConnectionFilter(bannedIp));
        }

        public void AddAllowLocalInterfaceIpConnectionFilter(String allowedIp)
        {
            _manager.RegisterConnectionFilter(new AllowLocalInterfaceIpConnectionFilter(allowedIp));
        }

        public void SetAuthCredidentials(String userName, String userPassword)
        {
            _manager.SetAuthCredidentials(userName, userPassword);
        }

        public void EnableAuthFilter(Boolean enable)
        {
            _manager.EnableAuthFilter(enable);
        }

        public void Dispose()
        {
            if (_manager != null)
            {
                _manager.Dispose();
                _manager = null;
            }
        }
    }
}
