using System;

namespace HttpServer.Filters.Security.Connection
{
    internal class RemoteBannedIpEqualsConnectionFilter : AbstractConnectionFilter
    {
        private String _bannedIp;

        public RemoteBannedIpEqualsConnectionFilter(String bannedIp)
        {
            _bannedIp = bannedIp;
        }

        protected override Boolean CheckConnectionInvalid(String localIp, String requestIp)
        {
            return _bannedIp.Equals(requestIp);
        }
    }
}
