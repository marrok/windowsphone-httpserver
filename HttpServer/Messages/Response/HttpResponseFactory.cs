using HttpServer.Messages.Response.Content;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpServer.Messages.Response
{
    public class HttpResponseFactory
    {
        private static readonly Dictionary<Type, Func<object, IHttpResponseContent>> RESPONSE_CONTENT_FACTORY_DICT = new Dictionary<Type, Func<object, IHttpResponseContent>>
        {
            { typeof(TextResponseContent),       (content) =>
                                                                new TextResponseContent { Content = (content as string) } },
            { typeof(JpgResponseContent),        (content) =>
                                                                new JpgResponseContent { Content = (content as byte[]) } },
            { typeof(PngResponseContent),        (content) =>
                                                                new PngResponseContent { Content = (content as byte[]) } },
            { typeof(JavaScriptResourceContent), (content) =>
                                                                new JavaScriptResourceContent { Content = (content as byte[]) } },
            { typeof(CssResponseContent),        (content) =>
                                                                new CssResponseContent { Content = (content as byte[]) } },
            { typeof(GifResponseContent),        (content) =>
                                                                new GifResponseContent { Content = (content as byte[]) } },
            { typeof(WavResponseContent),        (content) =>
                                                                new WavResponseContent { Content = (content as byte[]) } },
            { typeof(JNLPResponseContent),       (content) =>
                                                                new JNLPResponseContent { Content = (content as string) } },
            { typeof(Mp3ResponseContent),       (content) =>
                                                                new Mp3ResponseContent { Content =  (content as byte[]) } },
            { typeof(JarResponseContent),       (content) =>
                                                                new JarResponseContent { Content =  (content as byte[]) } },
            { typeof(HtmlResponseContent),       (content) =>
        {
            var resopnseContent = content is byte[] ?
                                                                                            content as byte[] :
                                                                                            Encoding.UTF8.GetBytes(content as string);
            return new HtmlResponseContent { Content = resopnseContent };
        } }
        };

        private static readonly Dictionary<HttpStatusCode, String> STATIC_RESPONSE_HTML_DICT = new  Dictionary<HttpStatusCode, String>
        {
            { HttpStatusCode.NotFound,  "<html><head><title>Upss did not find what you have been looking for</title></head><body><h1>Upss did not find what you have been looking for</h1><h1>Server error 404<h1></body></html>" },
            { HttpStatusCode.TooManyRequests, "<html><head><title>Upss there is too many requests for that resource</title></head><body><h1>Upss there is too many requests for that resource</h1><h1>Server error 429<h1></body></html>" },
            { HttpStatusCode.Unauthorized, "<html><head><title>Unauthorized access</title></head><body><h1>Unauthorized access</h1><h1>Server error 401<h1></body></html>" },
            { HttpStatusCode.Forbidden, "<html><head><title>Access Forbidden</title></head><body><h1>Forbidden</h1><h1>Access Forbidden<h1></body></html>" },
            { HttpStatusCode.ResourceInUse, "<html><head><title>Upss requested resource is already in use</title></head><body><h1>Upss there is too many requests for that resource</h1><h1>Server error 423<h1></body></html>" }
        };

        public static HttpResponse CreateResponse(HttpStatusCode statusCode, object content, Type ContentType)
        {
            var response = new HttpResponse() { StatusCode = statusCode, Content = RESPONSE_CONTENT_FACTORY_DICT[ContentType](content) };
            return response;
        }

        public static HttpResponse CreateHttpInternalServerErrorResponse(Exception e)
        {
            var html = new StringBuilder();

            html.Append("<html><head><title>Upss something went wrong</title></head><body><h1>Server failed. Maybe next time</h1><h1>Server error 500<h1><h2>");
            html.Append(e.Message).Append("</h2><br/>").Append(e.ToString().Replace("\n", "<br/>")).Append("</body></html>");

            var response = new HttpResponse() { StatusCode = HttpStatusCode.InternalServerError, Content = new HtmlResponseContent { Content = Encoding.UTF8.GetBytes(html.ToString()) } };

            return response;
        }

        private static HttpResponse CreateHttpResonse(HttpStatusCode status)
        {
            var response = new HttpResponse() { StatusCode = status, Content = new HtmlResponseContent { Content = Encoding.UTF8.GetBytes(STATIC_RESPONSE_HTML_DICT[status]) } };

            return response;
        }

        public static HttpResponse CreateHttpNotFoundResponse()
        {
            return CreateHttpResonse(HttpStatusCode.NotFound);
        }

        public static HttpResponse CreateHttpTooManyRequestsResponse()
        {
            return CreateHttpResonse(HttpStatusCode.TooManyRequests);
        }

        public static HttpResponse CreateHttpUnauthorizedResponse()
        {
            var response = CreateHttpResonse(HttpStatusCode.Unauthorized);
            response.Headers.Add(HttpHeaders.AUTHENTICATE_HEADER, "Basic realm=\"Windows Phone Server Authorization Required\"");
            return response;
        }

        public static HttpResponse CreateHttpForbiddenResponse()
        {
            return CreateHttpResonse(HttpStatusCode.Forbidden);
        }

        public static HttpResponse CreateHttp423Response()
        {
            return CreateHttpResonse(HttpStatusCode.ResourceInUse);
        }
    }
}
