namespace HttpServer.Messages.Response.Content
{
    public class GifResponseContent : BaseFileResourceContent
    {
        protected override string GetContentType()
        {
            return "image/gif";
        }
    }
}
