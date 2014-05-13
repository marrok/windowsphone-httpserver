namespace HttpServer.Messages.Response.Content
{
    public class HtmlResponseContent : BaseFileResourceContent
    {
        protected override string GetContentType()
        {
            return "text/html; charset=utf-8";
        }
    }
}
