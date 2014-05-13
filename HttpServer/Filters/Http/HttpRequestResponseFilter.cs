using HttpServer.Messages.Request;
using HttpServer.Messages.Response;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace HttpServer.Filters.Http
{
    internal class HttpRequestResponseFilter : Filter
    {
        private static readonly uint MAX_REQUEST_SIZE = 1024 * 1024 * 24;
        private AbstractHttpFilter _filter;

        internal override async Task DoFilter(Windows.Networking.Sockets.StreamSocket socket)
        {
            var response = new HttpResponse();
            try
            {
                HttpRequest request;

                using (var reader = new DataReader(socket.InputStream))
                {
                    var requestParser = new HttpRequestParser();
                    reader.InputStreamOptions = InputStreamOptions.Partial;

                    await reader.LoadAsync(MAX_REQUEST_SIZE);
                    var requestBuffer = new byte[reader.UnconsumedBufferLength];
                    reader.ReadBytes(requestBuffer);

                    request = requestParser.Parse(socket.Information, requestBuffer);
                    await _filter.DoFilter(request, response);
                }
            }
            catch (Exception e)
            {
                _logger.Error("Something went wrong: {0}", e.Message.ToString());
                response = HttpResponseFactory.CreateHttpInternalServerErrorResponse(e);
            }

            using (var writer = new DataWriter(socket.OutputStream))
            {
                writer.WriteBytes(response.CreateResponse().ToArray());
                await writer.StoreAsync();
            }
        }

        internal void AddHttpFilter(AbstractHttpFilter filter)
        {
            if (_filter == null)
            {
                _filter = filter;
            }
            else
            {
                filter.SetNext(_filter);
                _filter = filter;
            }
        }
    }
}
