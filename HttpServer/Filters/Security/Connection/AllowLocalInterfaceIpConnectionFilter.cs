using System;

namespace HttpServer.Filters.Security.Connection
{
    internal class AllowLocalInterfaceIpConnectionFilter : AbstractConnectionFilter
    {
        private String _allowedIp;

        public AllowLocalInterfaceIpConnectionFilter(String allowedIp)
        {
            _allowedIp = allowedIp;
        }

        protected override Boolean CheckConnectionInvalid(String localIp, String requestIp)
        {
            return !_allowedIp.Equals(localIp);
        }
    }
}
