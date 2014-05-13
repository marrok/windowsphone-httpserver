using System;

namespace HttpServer.Filters.Security.Connection
{
    internal class LocalBannedIpEqualsConnectionFilter : AbstractConnectionFilter
    {
        private String _bannedIp;

        public LocalBannedIpEqualsConnectionFilter(String bannedIp)
        {
            _bannedIp = bannedIp;
        }

        protected override Boolean CheckConnectionInvalid(String localIp, String requestIp)
        {
            return _bannedIp.Equals(localIp);
        }
    }
}
