namespace HttpServer.Messages.Response.Content
{
    public class PngResponseContent : BaseFileResourceContent
    {
        protected override string GetContentType()
        {
            return "image/png";
        }
    }
}
