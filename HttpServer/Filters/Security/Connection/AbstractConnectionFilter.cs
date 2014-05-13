using System;

namespace HttpServer.Filters.Security.Connection
{
    internal abstract class AbstractConnectionFilter
    {
        protected abstract Boolean CheckConnectionInvalid(String localIp, String requestIp);

        private AbstractConnectionFilter _next;

        internal void SetNext(AbstractConnectionFilter next)
        {
            _next = next;
        }

        public Boolean DropConnection(String localIp, String requestIp)
        {
            return CheckConnectionInvalid(localIp, requestIp) ?
                        true :
                        (_next != null ? _next.DropConnection(localIp, requestIp) : false);
        }
    }
}
