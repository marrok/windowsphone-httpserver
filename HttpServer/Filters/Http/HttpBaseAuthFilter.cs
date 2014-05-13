using HttpServer.Messages.Request;
using HttpServer.Messages.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpServer.Filters.Http
{
    internal class HttpBaseAuthFilter : AbstractHttpFilter, IDisposable
    {
        private HashSet<String> _authorizedIpAddresses = new HashSet<String>();
        private String _userCredidentials;
        private Semaphore _semaphore = new Semaphore(1, 1);
        private volatile Boolean _authEnabled;

        private static readonly int BASIC_HTTP_AUTH_DROP_LENGTH = "basic ".Length;

        internal void SetUserCredidentials(String userName, String userPassword)
        {
            _userCredidentials = userName + ":" + userPassword;
        }

        internal void EnableBasicAuth(Boolean enable)
        {
            _semaphore.WaitOne();
            _authEnabled = enable;
            if (!enable)
            {
                _authorizedIpAddresses.Clear();
            }
            _semaphore.Release();
        }

        internal override async Task DoFilter(HttpRequest request, HttpResponse response)
        {
            if (!_authEnabled || ValidateIpAddress(request.RemoteHostIp))
            {
                await DoChain(request, response);
            }
            else
            {
                if (request.Headers.ContainsKey(HttpHeaders.BASIC_AUTH_HEADER))
                {
                    var data = Convert.FromBase64String(request.Headers[HttpHeaders.BASIC_AUTH_HEADER].Remove(0, BASIC_HTTP_AUTH_DROP_LENGTH));
                    var decodedString = Encoding.UTF8.GetString(data, 0, data.Length);

                    if (decodedString.Equals(_userCredidentials))
                    {
                        AddAuthenticatedAddress(request.RemoteHostIp);
                        await DoChain(request, response);
                    }
                    else
                    {
                        _logger.Warning("Received wrong user credentials from {0}", request.RemoteHostIp);
                        DeepCopyResponse(response, HttpResponseFactory.CreateHttpForbiddenResponse());
                    }
                }
                else
                {
                    _logger.Warning("Received request from {0}, sending authorization request", request.RemoteHostIp);
                    DeepCopyResponse(response, HttpResponseFactory.CreateHttpUnauthorizedResponse());
                }
            }
        }

        private Boolean ValidateIpAddress(String ipAddres)
        {
            _semaphore.WaitOne();
            var isValid = _authorizedIpAddresses.Contains(ipAddres);
            _semaphore.Release();

            return isValid;
        }

        private void AddAuthenticatedAddress(String ipAddres)
        {
            _semaphore.WaitOne();

            if (!_authorizedIpAddresses.Contains(ipAddres))
            {
                _authorizedIpAddresses.Add(ipAddres);
            }

            _semaphore.Release();
        }

        public void Dispose()
        {
            _semaphore.Dispose();
        }
    }
}
