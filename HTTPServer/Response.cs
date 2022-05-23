using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        StatusCode code;
        List<string> headerLines = new List<string>();


        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }

        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            headerLines.Add("Content-Type: " + contentType + "\r\n");
            headerLines.Add("Content-length: " + content.Length.ToString() + "\r\n");
            headerLines.Add("Date: " + DateTime.Now.ToString() + "\r\n");
            if (!string.IsNullOrEmpty(redirectoinPath))
            {
                headerLines.Add("Location: " + redirectoinPath + "\r\n");
            }
            string statusLine = GetStatusLine(code);

            // TODO: Create the response string
            responseString = statusLine;
            foreach (string line in headerLines)
                responseString += line;
            responseString += "\r\n" + content;

        }

        private string GetStatusLine(StatusCode code)
        {

            // TODO: Create the response status line and return it
            int statuscode = (int)code;
            string statusLine = Configuration.ServerHTTPVersion + " " + statuscode.ToString() + " " + code.ToString();
            return statusLine;
        }
    }
}