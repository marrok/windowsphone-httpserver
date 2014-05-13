using HttpServer.Logger;
using HttpServer.Messages.Request;
using HttpServer.Messages.Response;
using System.Threading.Tasks;

namespace HttpServer.Filters.Http
{
    internal abstract class AbstractHttpFilter
    {
        private AbstractHttpFilter _next;
        protected SimpleLogger _logger = SimpleLogger.GetLogger();

        internal void SetNext(AbstractHttpFilter next)
        {
            _next = next;
        }

        internal abstract Task DoFilter(HttpRequest request, HttpResponse response);

        internal async Task DoChain(HttpRequest request, HttpResponse response)
        {
            if (_next != null)
            {
                await _next.DoFilter(request, response);
            }
        }

        protected void DeepCopyResponse(HttpResponse baseResponse, HttpResponse responseToCopy)
        {
            baseResponse.Content = responseToCopy.Content;
            baseResponse.Headers = responseToCopy.Headers;
            baseResponse.StatusCode = responseToCopy.StatusCode;
        }
    }
}
