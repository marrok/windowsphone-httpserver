using Windows.Networking.Connectivity;

namespace HttpServer.Helpers
{
    public class IpAddressHelper
    {
        private static readonly uint WIFI_IANA_ID = 71;

        public static string FetchIpAddress()
        {
            string ipAddresses = null;

            foreach (var hn in NetworkInformation.GetHostNames())
            {
                if (hn.IPInformation != null && hn.IPInformation.NetworkAdapter.IanaInterfaceType == WIFI_IANA_ID)
                {
                    ipAddresses = hn.DisplayName;
                }
            }

            return ipAddresses;
        }
    }
}
