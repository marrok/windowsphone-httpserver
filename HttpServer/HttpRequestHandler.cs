using HttpServer.Messages.Request;
using HttpServer.Messages.Response;
using System;
using System.Text.RegularExpressions;

namespace HttpServer
{
    public delegate HttpResponse HttpRequestHandlerDelegate(HttpRequest request);

    internal class HttpRequestHandler
    {
        public String HandledUri { get; set; }
        public Boolean HandleUriWithRegexp { get; set; }
        public HttpRequestType SupportedRequestType { get; set; }
        public HttpRequestHandlerDelegate HandlerDelegate { get; set; }

        private Regex _requestRegex;

        public void BuildHandler()
        {
            if (HandleUriWithRegexp)
            {
                _requestRegex = new Regex(HandledUri);
            }
        }

        public Boolean CanHandleRequest(HttpRequest request)
        {
            return SupportedRequestType.Equals(request.RequestType) && (HandleUriWithRegexp ? _requestRegex.IsMatch(request.Uri) : HandledUri.Equals(request.Uri));
        }

        public HttpResponse HandleRequest(HttpRequest request)
        {
            return HandlerDelegate(request);
        }
    }
}
