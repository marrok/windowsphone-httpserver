namespace HttpServer.Messages.Response.Content
{
    public class WavResponseContent : BaseFileResourceContent
    {
        protected override string GetContentType()
        {
            return "audio/wav";
        }
    }
}
