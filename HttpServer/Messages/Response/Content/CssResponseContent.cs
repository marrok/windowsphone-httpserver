namespace HttpServer.Messages.Response.Content
{
    public class CssResponseContent : BaseFileResourceContent
    {
        protected override string GetContentType()
        {
            return "text/css";
        }
    }
}
