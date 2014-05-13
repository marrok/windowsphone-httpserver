using System.Collections.Generic;

namespace HttpServer.Messages.Request.Content
{
    internal class FormRequestContent : IHttpRequestContent
    {
        public Dictionary<string, string> FormData { get; set; }
    }
}
