using HttpServer.Messages;
using HttpServer.Messages.Request;
using HttpServer.Messages.Response;
using HttpServer.Messages.Response.Content;
using System;
using System.Collections.Generic;
using System.Windows;

namespace HttpServer.StaticResource
{
    internal class StaticResourceManager
    {
        private static readonly Dictionary<StaticResourceType, Type> RESOURCES_TYPE_MAP = new Dictionary<StaticResourceType, Type>
        {
             { StaticResourceType.JPG,  typeof(JpgResponseContent) },
             { StaticResourceType.PNG, typeof(PngResponseContent) },
             { StaticResourceType.JAVA_SCRIPT, typeof(JavaScriptResourceContent) },
             { StaticResourceType.HTML, typeof(HtmlResponseContent) },
             { StaticResourceType.CSS, typeof(CssResponseContent) },
             { StaticResourceType.WAV, typeof(WavResponseContent) },
             { StaticResourceType.MP3, typeof(Mp3ResponseContent) },
             { StaticResourceType.JAR, typeof(JarResponseContent) },
             { StaticResourceType.GIF, typeof(GifResponseContent) }
        };

        internal static HttpRequestHandler CreateStaticResourceHandler(string resourcePath, string resourceUri, StaticResourceType type)
        {
            var handler = new HttpRequestHandler
            { HandledUri = resourceUri,
                HandleUriWithRegexp = false,
                SupportedRequestType = HttpRequestType.GET,
                HandlerDelegate = request =>
            {
                var resourceInfo = Application.GetResourceStream(new Uri(resourcePath, UriKind.Relative));
                var buffer = new byte[resourceInfo.Stream.Length];
                resourceInfo.Stream.Read(buffer, 0, buffer.Length);

                var responseType = RESOURCES_TYPE_MAP[type];

                return HttpResponseFactory.CreateResponse(HttpStatusCode.Ok, buffer, responseType);
            } };

            handler.BuildHandler();
            return handler;
        }
    }
}
