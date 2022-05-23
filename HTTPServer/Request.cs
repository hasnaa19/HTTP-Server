using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            //throw new NotImplementedException();

            //TODO: parse the receivedRequest using the \r\n delimeter   

            string[] stringSeparators = new string[] { "\r\n" };
            string[] REQSTRINGLINES = requestString.Split(stringSeparators, StringSplitOptions.None);

            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (REQSTRINGLINES.Length < 3)
            {
                return false;
            }
            // Parse Request line
            if (ParseRequestLine(REQSTRINGLINES) == false)
            {
                return false;
            }
            // Validate blank line exists
            if (!ValidateBlankLine(REQSTRINGLINES))
            {
                return false;
            }
            // Load header lines into HeaderLines dictionary

            string[] HL = new string[REQSTRINGLINES.Length - 3];//REQSTRINGLENGTH-(REQLINE,BLANKLINE,CONTENT)

            int count = 0;
            for (int i = 1; i < REQSTRINGLINES.Length - 2; i++)//hmshy mn b3d l reqline lhd abl l blank line
            {
                HL[count] = REQSTRINGLINES[i];
                count++;
            }

            if (LoadHeaderLines(HL) == false)
            {
                return false;
            }


            return true;
        }

        private bool ParseRequestLine(string[] REQSTRINGLINES)
        {
            string[] stringSeparators = new string[] { " " };

            requestLines = REQSTRINGLINES[0].Split(stringSeparators, StringSplitOptions.None);
            if (requestLines.Length < 3)//Method uri httpversion
            {
                return false;
            }
            method = (RequestMethod)Enum.Parse(typeof(RequestMethod), requestLines[0]);
            relativeURI = requestLines[1].Substring(1);
            if (ValidateIsURI(relativeURI) == false)
            {
                return false;
            }
            if (requestLines[2] == "HTTP/0.9")
            {
                httpVersion = HTTPVersion.HTTP09;
            }
            else if (requestLines[2] == "HTTP/1.0")
            {
                httpVersion = HTTPVersion.HTTP10;
            }
            else if (requestLines[2] == "HTTP/1.1")
            {
                httpVersion = HTTPVersion.HTTP11;
            }
            return true;
            // throw new NotImplementedException();
        }

        private bool ValidateIsURI(string uri)//byshof l uri mktob btry2a sa7 wla la 
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines(string[] HL)
        {
            if (HL.Length < 1)
            {
                return false;
            }

            headerLines = new Dictionary<string, string>();

            foreach (string line in HL)
            {
                int index = 0;
                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i] == ':')
                    {
                        index = i;
                        break;
                    }
                }
                headerLines.Add(line.Substring(0, index), line.Substring(index + 1));
            }
            return true;
            //throw new NotImplementedException();
        }

        private bool ValidateBlankLine(string[] REQSTRINGLINES)
        {
            if (REQSTRINGLINES[REQSTRINGLINES.Length - 2] == "")
            {
                return true;
            }
            else
            {
                return false;
            }
            //throw new NotImplementedException();
        }

    }
}
