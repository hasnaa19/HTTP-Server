using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
         
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint EP = new IPEndPoint(IPAddress.Any, portNumber);
            this.serverSocket.Bind(EP);
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            serverSocket.Listen(1000);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clientsocket = serverSocket.Accept();
                Thread newThread = new Thread(new ParameterizedThreadStart(HandleConnection));
                newThread.Start(clientsocket);
            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            Socket clientsocket = (Socket)obj;
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            clientsocket.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.
            int reqdatalength;
            while (true)
            {
                try
                {

                    // TODO: Receive request
                    byte[] Requestdata = new byte[1024*1024];
                    reqdatalength = clientsocket.Receive(Requestdata);

                    // TODO: break the while loop if receivedLen==0
                    if (reqdatalength == 0)
                    {
                        Console.WriteLine("Client: {0} ended the connection", clientsocket.RemoteEndPoint);
                        break;

                    }
                
                    // TODO: Create a Request object using received request string
                    string requestString = Encoding.ASCII.GetString(Requestdata, 0, reqdatalength);

                    Request Req = new Request(requestString);//"GET /~hollingd/testanswers.html HTTP/1.1"  ****************************FOR BAD REQUEST***********************************
                    // TODO: Call HandleRequest Method that returns the response
                    Response Res=HandleRequest(Req);
                    // TODO: Send Response back to client
                    clientsocket.Send(Encoding.ASCII.GetBytes(Res.ResponseString));
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            clientsocket.Close();
        }

        Response HandleRequest(Request request)
        {
            //throw new NotImplementedException();
            string content;

            try
            {
                //​throw new NotImplementedException("Internal Server Error"); 
                //TODO: check for bad request 
                if (!request.ParseRequest())
                {
                    Response Bad_req = new Response(StatusCode.BadRequest, "text/html", LoadDefaultPage(Configuration.BadRequestDefaultPageName), "");
                    return Bad_req;
                }

                //TODO: map the relativeURI in request to get the physical path of the resource.
                string Physical_Path = Path.Combine(Configuration.RootPath, request.relativeURI);
                //TODO: check for redirect
                string redirectionPath = GetRedirectionPagePathIFExist(request.relativeURI);
                if (redirectionPath != "")
                {
                    Response Redirect = new Response(StatusCode.Redirect, "text/html", LoadDefaultPage(Configuration.RedirectionDefaultPageName), redirectionPath);
                    return Redirect;
                }
                //TODO: check file exists
                if (!File.Exists(Physical_Path))
                {
                    Response NotFound = new Response(StatusCode.NotFound, "text/html", LoadDefaultPage(Configuration.NotFoundDefaultPageName), "");
                    return NotFound;
                }

                //TODO: read the physical file
                StreamReader reader = new StreamReader(Physical_Path);
                string file = reader.ReadToEnd();
                reader.Close();
                // Create OK response
                return new Response(StatusCode.OK, "text/html", file, "");

            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error.
                Response ISE = new Response(StatusCode.InternalServerError, "text/html",
                    LoadDefaultPage(Configuration.InternalErrorDefaultPageName), "");

                return ISE;

            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            if (Configuration.RedirectionRules.ContainsKey(relativePath))
            {
                return Configuration.RedirectionRules[relativePath];
            }
            else
            return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);            //"C:\inetpub\wwwroot\fcis1\aboutus2.html"for example
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            try { 
                string filecontent = System.IO.File.ReadAllText(filePath);
                return filecontent;
            }
            // else read file and return its content
            catch(Exception ex)
            {
                Logger.LogException(ex);
                return string.Empty;
            }
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                string[] lines = File.ReadAllLines(filePath);
                // then fill Configuration.RedirectionRules dictionary
                Configuration.RedirectionRules = new Dictionary<string, string>();
                foreach (string line in lines)
                {
                    int index = 0;
                    for (int i = 0; i < line.Length; i++)
                    {
                        if (line[i] == ',')
                        {
                            index = i;
                            break;
                        }
                    }
                    Configuration.RedirectionRules.Add(line.Substring(0, index), line.Substring(index + 1));
                }
                //foreach (KeyValuePair<string, string> rule in Configuration.RedirectionRules)
                //{
                //    Console.WriteLine("Key: {0}, Value: {1}", rule.Key, rule.Value);
                //}
                //Console.ReadLine();


            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}