using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpServer.Security
{
    internal class ConnectionFilter
    {
        private AbstractConnectionFilter _filter;
        private volatile Boolean _filterConnections = true;
        private Semaphore _semaphore = new Semaphore(1, 1);

        public void EnableFiltering(Boolean enable)
        {
            _filterConnections = enable;
        }
        
        private void AddConnectionFilter(AbstractConnectionFilter filter)
        {
            if (_filter == null)
            {
                _filter = filter;
            }
            else
            {
                filter.Next = _filter.Next;
                _filter.Next = filter;
            }
        }

        public Boolean DropConnection(String localIp, String requestIp)
        {
            return _filterConnections  && _filter != null? _filter.DropConnection(localIp, requestIp) : false;
        }

        public void AddRemoteIpEqualsConnectionFilter(String bannedIp)
        {
            AddConnectionFilter(new RemoteIpEqualsConnectionFilter(bannedIp));
        }

        public void AddLocalIpEqualsConnectionFilter(String bannedIp)
        {
            AddConnectionFilter(new LocalIpEqualsConnectionFilter(bannedIp));
        }

        public void AddAllowLocalInterfaceIpConnectionFilter(String allowedIp)
        {
            AddConnectionFilter(new AllowLocalInterfaceIpConnectionFilter(allowedIp));
        }

        private abstract class AbstractConnectionFilter
        {

            protected abstract Boolean CheckConnectionInvalid(String localIp, String requestIp);

            public AbstractConnectionFilter Next { get; set; }

            public Boolean DropConnection(String localIp, String requestIp)
            {
                return CheckConnectionInvalid(localIp, requestIp) ?
                            true :
                            (Next != null ? Next.DropConnection(localIp, requestIp) : false);
            }
        }

        private class RemoteIpEqualsConnectionFilter : AbstractConnectionFilter
        {
            private String _bannedIp;

            public RemoteIpEqualsConnectionFilter(String bannedIp)
            {
                this._bannedIp = bannedIp;
            }

            override protected Boolean CheckConnectionInvalid(String localIp, String requestIp)
            {
                return _bannedIp.Equals(requestIp);
            }
        }

        private class LocalIpEqualsConnectionFilter : AbstractConnectionFilter
        {
            private String _bannedIp;

            public LocalIpEqualsConnectionFilter(String bannedIp)
            {
                this._bannedIp = bannedIp;
            }

            override protected Boolean CheckConnectionInvalid(String localIp, String requestIp)
            {
                return _bannedIp.Equals(localIp);
            }
        }

        private class AllowLocalInterfaceIpConnectionFilter : AbstractConnectionFilter
        {
            private String _allowedIp;

            public AllowLocalInterfaceIpConnectionFilter(String allowedIp)
            {
                this._allowedIp = allowedIp;
            }

            override protected Boolean CheckConnectionInvalid(String localIp, String requestIp)
            {
                return !_allowedIp.Equals(localIp);
            }
        }
    }
}
