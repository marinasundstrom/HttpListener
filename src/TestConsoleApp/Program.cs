using System;
using System.Net;
using System.Net.Http;

namespace TestConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int port = 18081;

            var listener = new HttpListener(IPAddress.Any, port);
            listener.Request += HandleRequest;
            listener.Start();

            bool isListening = listener.IsListening;
            Console.WriteLine("isListening = {0}", isListening);

            Console.WriteLine("Press any key to stop listener");
            
            Console.ReadKey();
            listener.Close();
            listener.Dispose();
        }

        private static async void HandleRequest(object sender, HttpListenerRequestEventArgs e)
        {
            var request = e.Request;
            var response = e.Response;

            if (request.Method == HttpMethods.Get)
            {
                string content = @"<h2>Hello! What's your name?</h2>
                                <form method=""POST"" action=""/?test=2"">
                                    <input name=""name""></input>
                                    <button type=""submit"">Send</button>
                                </form>";

                await response.WriteContentAsync(MakeDocument(content));
            }
            else if (request.Method == HttpMethods.Post)
            {
                var param = request.RequestUri.ParseQueryParameters();

                var data = await request.ReadUrlEncodedContentAsync();
                var name = data["name"];

                string content = $"<h2>Hi, {name}! Nice to meet you.</h2>";

                await response.WriteContentAsync(MakeDocument(content));

                Console.WriteLine($"--> Hi, {name}! Nice to meet you.");
            }
            else
            {
                response.MethodNotAllowed();
            }

            response.Close();
        }

        private static string MakeDocument(object content)
        {
            return @"<html>
                        <head>
                            <title>Test</title>
                        </head>
                        <body>" +
                            content +
                        @"</body>
                    </html>";
        }
    }
}
