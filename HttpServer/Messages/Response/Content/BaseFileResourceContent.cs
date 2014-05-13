using System.Collections.Generic;

namespace HttpServer.Messages.Response.Content
{
    public abstract class BaseFileResourceContent : IHttpResponseContent
    {
        internal byte[] Content { get; set; }

        public byte[] SerializeContent()
        {
            return Content;
        }

        public Dictionary<string, string> GetAdditionalHeaders()
        {
            var dict = new Dictionary<string, string>();
            dict.Add("Content-Type", GetContentType());

            if (Content != null)
            {
                dict.Add("Content-Length", Content.Length.ToString());
            }
            return dict;
        }

        protected abstract string GetContentType();
    }
}
