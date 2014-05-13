using HttpServer.Exceptions;
using HttpServer.Messages.Request.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Windows.Networking.Sockets;

namespace HttpServer.Messages.Request
{
    internal class HttpRequestParser
    {
        internal HttpRequest Parse(StreamSocketInformation socketInfo, byte[] requestBytes)
        {
            var requestAsText = Encoding.UTF8.GetString(requestBytes, 0, requestBytes.Length);

            var httpReqest = CreateHttpRequest(requestAsText);
            httpReqest.Content = GetRequestContnet(httpReqest.RequestType, requestAsText);
            httpReqest.RemoteHostIp = socketInfo.RemoteAddress.DisplayName;

            return httpReqest;
        }

        private IHttpRequestContent GetRequestContnet(HttpRequestType requestType, string requestString)
        {
            if (HttpRequestType.GET.Equals(requestType))
            {
                var requestTypeHeaderEnd = requestString.IndexOf("\r\n");
                var requestTypeHeader = requestString.Substring(0, requestTypeHeaderEnd);
                var typeHeaderParams = requestTypeHeader.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                return typeHeaderParams[1].Contains("?") ?
                    new FormRequestContent { FormData = ParseFormData(typeHeaderParams[1].Split(new char[] { '?' }, StringSplitOptions.RemoveEmptyEntries)[1]) } :
                    null;
            }
            else
            {
                if (HttpRequestType.POST.Equals(requestType))
                {
                    var requestHeaderEnd = requestString.IndexOf("\r\n\r\n") + 4;
                    var requestTypeHeader = requestString.Substring(requestHeaderEnd, requestString.Length - requestHeaderEnd);

                    return new FormRequestContent { FormData = ParseFormData(requestTypeHeader) };
                }
            }
            throw new HttpException("Unsupported request type");
        }

        private Dictionary<string, string> ParseFormData(string formDataString)
        {
            var formData = new Dictionary<string, string>();

            foreach (var data in formDataString.Split('&'))
            {
                var keyValuePair = data.Split('=');
                formData.Add(keyValuePair[0], keyValuePair[1]);
            }

            return formData;
        }

        private HttpRequest CreateHttpRequest(string request)
        {
            var httpRequest = new HttpRequest();

            var headersEndIndex = request.IndexOf("\r\n\r\n");
            var headersString = headersEndIndex != -1 ? request.Substring(0, headersEndIndex) : request;
            var headersList = headersString.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            if (headersList.Any())
            {
                var requestTypeParams = headersList[0].Split(' ');
                httpRequest.RequestType = GetRequestType(requestTypeParams[0]);
                httpRequest.Uri = HttpUtility.UrlDecode(requestTypeParams[1].Split('?')[0]);

                foreach (var header in headersList.Skip(1))
                {
                    var headerSplitIndex = header.IndexOf(":");
                    var headerName = header.Substring(0, headerSplitIndex);
                    var headerValue = header.Substring(headerSplitIndex + 1);

                    httpRequest.Headers.Add(headerName, headerValue);
                }
            }

            return httpRequest;
        }

        private HttpRequestType GetRequestType(string requestTypeString)
        {
            if ("GET".Equals(requestTypeString))
            {
                return HttpRequestType.GET;
            }
            else
            {
                if ("POST".Equals(requestTypeString))
                {
                    return HttpRequestType.POST;
                }
                else
                {
                    throw new HttpException("Unsupported request type");
                }
            }
        }
    }
}
