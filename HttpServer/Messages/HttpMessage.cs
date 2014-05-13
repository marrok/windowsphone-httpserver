using System.Collections.Generic;

namespace HttpServer.Messages
{
    public abstract class HttpMessage
    {
        protected HttpMessage()
        {
            Headers = new Dictionary<string, string>();
        }

        internal Dictionary<string, string> Headers { get; set; }
    }
}
