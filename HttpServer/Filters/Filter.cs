using HttpServer.Logger;
using System.Threading.Tasks;
using Windows.Networking.Sockets;

namespace HttpServer.Filters
{
    internal abstract class Filter
    {
        private Filter _next;
        protected SimpleLogger _logger = SimpleLogger.GetLogger();

        internal void SetNext(Filter next)
        {
            _next = next;
        }

        internal abstract Task DoFilter(StreamSocket socket);

        internal async Task DoChain(StreamSocket socket)
        {
            if (_next != null)
            {
                await _next.DoFilter(socket);
            }
        }
    }
}
