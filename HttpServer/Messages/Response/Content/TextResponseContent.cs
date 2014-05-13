namespace HttpServer.Messages.Response.Content
{
    public class TextResponseContent : BaseTextResponseContent
    {
        protected override string GetContentType()
        {
            return "text/plain; charset=utf-8";
        }
    }
}
