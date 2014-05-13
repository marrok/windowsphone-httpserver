using System;

namespace HttpServer.Exceptions
{
    internal class HttpException : Exception
    {
        public HttpException(string message)
            : base(message)
        {
        }
    }
}
