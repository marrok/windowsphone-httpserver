namespace HttpServer.Messages.Response.Content
{
    public class JpgResponseContent : BaseFileResourceContent
    {
        protected override string GetContentType()
        {
            return "image/jpg";
        }
    }
}
