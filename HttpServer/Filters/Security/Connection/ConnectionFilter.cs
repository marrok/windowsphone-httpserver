using System;
using System.Threading.Tasks;
using Windows.Networking.Sockets;

namespace HttpServer.Filters.Security.Connection
{
    internal class ConnectionFilter : Filter
    {
        private AbstractConnectionFilter _filter;
        private volatile Boolean _filterConnections = true;

        internal override async Task DoFilter(StreamSocket socket)
        {
            if (!DropConnection(socket.Information.LocalAddress.DisplayName, socket.Information.RemoteAddress.DisplayName))
            {
                await DoChain(socket);
            }
            else
            {
                _logger.Warning("Rejected connection from {0}", socket.Information.RemoteAddress.DisplayName);
            }
        }

        internal void EnableFiltering(Boolean enable)
        {
            _filterConnections = enable;
        }

        internal void AddConnectionFilter(AbstractConnectionFilter filter)
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

        internal Boolean DropConnection(String localIp, String requestIp)
        {
            return _filterConnections && _filter != null ? _filter.DropConnection(localIp, requestIp) : false;
        }
    }
}
