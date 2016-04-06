using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private HttpListener listener;

        public Uri Url { get; private set; }

        public MainPage()
        {
            this.InitializeComponent();

            Initialize();
        }

        private void Initialize()
        {
            var address = "192.168.1.100";
            var port = 8081;

            Url = (new UriBuilder($"http://{address}:{port}/test")).Uri;

            listener = new HttpListener(IPAddress.Parse(address), port);
            listener.Request += requestHandler;
            listener.Start();
        }

        private async void requestHandler(object sender, HttpListenerRequestEventArgs e)
        {
            var request = e.Request;
            var response = e.Response;

            if (request.Method == HttpMethods.Get)
            {
                var content = @"<h2>Hello! What's your name?</h2>
                                <form method=""POST"">
                                    <input name=""name""></input>
                                    <button type=""submit"">Send</button>
                                </form>";

                await response.WriteAsync(MakeDocument(content));
            }
            else if (request.Method == HttpMethods.Post)
            {
                string content = null;
                using (var streamReader = new StreamReader(request.InputStream))
                {
                    content = await streamReader.ReadToEndAsync();
                }

                //await response.Redirect(Url);

                var data = HttpUtility.ParseQueryString(content);

                var name = HttpUtility.UrlDecode(data["name"]);

                content = $"<h2>Hi, {name}! Nice to meet you.</h2>";

                await response.WriteAsync(MakeDocument(content));

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    var dialog = new MessageDialog($"Hi, {name}! Nice to meet you.");
                    await dialog.ShowAsync();
                });
            }
            else
            {
                response.MethodNotAllowed();
            }

            response.Close();
        }

        private string MakeDocument(object content)
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

        ~MainPage()
        {
            listener.Close();
        }
    }
}
