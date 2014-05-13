using System.Collections.Generic;

namespace HttpServer.Messages.Response.Content
{
    public interface IHttpResponseContent
    {
        byte[] SerializeContent();

        Dictionary<string, string> GetAdditionalHeaders();
    }
}
