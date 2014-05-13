using System;

namespace HttpServer.Exceptions
{
    internal class StreamingException : Exception
    {
        public StreamingException(string message)
            : base(message)
        {
        }
    }
}
