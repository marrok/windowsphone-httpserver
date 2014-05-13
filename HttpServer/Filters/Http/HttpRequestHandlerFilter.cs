using HttpServer.Messages.Request;
using HttpServer.Messages.Response;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpServer.Filters.Http
{
    internal class HttpRequestHandlerFilter : AbstractHttpFilter
    {
        internal List<HttpRequestHandler> _handlers = new List<HttpRequestHandler>();

        private HttpResponse HandleRequest(HttpRequest request)
        {
            var handler = _handlers.FirstOrDefault(h => h.CanHandleRequest(request));
            return handler != null ? handler.HandleRequest(request) : HttpResponseFactory.CreateHttpNotFoundResponse();
        }

        internal void RegisterHandler(HttpRequestHandler handler)
        {
            _handlers.Add(handler);
        }

        internal override async Task DoFilter(HttpRequest request, HttpResponse response)
        {
            _logger.Info("Connection received from {0} requesting {1}", request.RemoteHostIp, request.Uri);
            var handlerResponse = HandleRequest(request);
            DeepCopyResponse(response, handlerResponse);
            await DoChain(request, response);
        }
    }
}
