using HttpServer.Messages.Request.Content;
using System;

namespace HttpServer.Messages.Request
{
    public class HttpRequest : HttpMessage
    {
        public String Uri { get; internal set; }

        public HttpRequestType RequestType { get; internal set; }

        public IHttpRequestContent Content { get; internal set; }

        public String RemoteHostIp { get; internal set; }
    }
}
