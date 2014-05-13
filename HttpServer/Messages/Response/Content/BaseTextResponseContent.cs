using System.Collections.Generic;
using System.Text;

namespace HttpServer.Messages.Response.Content
{
    public abstract class BaseTextResponseContent : IHttpResponseContent
    {
        internal string Content { get; set; }

        public byte[] SerializeContent()
        {
            return Encoding.UTF8.GetBytes(Content);
        }

        public Dictionary<string, string> GetAdditionalHeaders()
        {
            return new Dictionary<string, string>()
            {
                { "Content-Type", GetContentType() },
                { "Content-Length", Content.Length.ToString() }
            };
        }

        protected abstract string GetContentType();
    }
}
