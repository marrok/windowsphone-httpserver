namespace HttpServer.Messages.Response.Content
{
    public class JavaScriptResourceContent : BaseFileResourceContent
    {
        protected override string GetContentType()
        {
            return "application/x-javascript";
        }
    }
}
