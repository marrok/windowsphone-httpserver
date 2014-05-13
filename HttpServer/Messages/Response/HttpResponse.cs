using HttpServer.Messages.Response.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpServer.Messages.Response
{
    public class HttpResponse : HttpMessage
    {
        internal IHttpResponseContent Content { get; set; }

        internal HttpStatusCode StatusCode { get; set; }

        public IEnumerable<byte> CreateResponse()
        {
            var headers = Encoding.UTF8.GetBytes(GetHeadersAndStatusString());
            var content = Content != null ? Content.SerializeContent() : new byte[0];

            return headers.Concat(content);
        }

        internal HttpResponse()
        {
            Headers.Add("Connection", "Keep-Alive");
            Headers.Add("Server", "Windows Phone 8 Custom Http Server");
            Headers.Add("Accept-Ranges", "bytes");
            Headers.Add("Date", DateTime.Now.ToUniversalTime().ToString("ddd, d MMM yyyy HH:mm:ss UTC"));
        }

        private String GetHeadersAndStatusString()
        {
            var headers = new Dictionary<string, string>();

            foreach (var header in Headers)
            {
                headers.Add(header.Key, header.Value);
            }

            if (Content != null)
            {
                foreach (var additionalHeader in Content.GetAdditionalHeaders())
                {
                    headers.Add(additionalHeader.Key, additionalHeader.Value);
                }
            }

            var sb = new StringBuilder();

            sb.Append("HTTP/1.1 ").Append((int)StatusCode).Append(" ")
                .Append(Enum.GetName(typeof(HttpStatusCode), StatusCode)).AppendLine();

            foreach (var header in headers)
            {
                sb.Append(header.Key).Append(": ").Append(header.Value).AppendLine();
            }

            sb.AppendLine();
            return sb.ToString();
        }
    }
}
