using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Messages.Response.Content
{
    class Mp3ResponseContent : BaseFileResourceContent
    {
        protected override string GetContentType()
        {
            return "audio/mp3";
        }
    }
}
