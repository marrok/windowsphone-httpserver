namespace HttpServer.Messages
{
    public enum HttpStatusCode
    {
        BadRequest = 400,
        Forbidden = 403,
        InternalServerError = 500,
        MethodNotAllowed = 405,
        NotAcceptable = 406,
        NotFound = 404,
        Ok = 200,
        PaymentRequired = 402,
        ResourceInUse = 423,
        TooManyRequests = 429,
        Unauthorized = 401
    }
}
