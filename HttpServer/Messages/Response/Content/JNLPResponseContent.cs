namespace HttpServer.Messages.Response.Content
{
    public class JNLPResponseContent : BaseTextResponseContent
    {
        protected override string GetContentType()
        {
            return "application/x-java-jnlp-file";
        }
    }
}
