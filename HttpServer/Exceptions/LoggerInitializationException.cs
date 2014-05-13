using System;

namespace HttpServer.Logger
{
    public class LoggerInitializationException : Exception
    {
        public LoggerInitializationException(string message)
            : base(message)
        {
        }
    }
}
