namespace HttpServer.Messages.Response.Content
{
    internal class JarResponseContent : BaseFileResourceContent
    {
        protected override string GetContentType()
        {
            return "application/x-java-jar-file";
        }
    }
}
