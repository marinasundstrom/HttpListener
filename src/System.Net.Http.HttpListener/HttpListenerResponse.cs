using System.Collections.Generic;
using System.IO;
using System.Net.Http.Abstractions;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Http
{
    public sealed class HttpListenerResponse : IDisposable
    {
        private TcpClientAdapter client;

        internal HttpListenerResponse(HttpListenerRequest request, TcpClientAdapter client)
        {
            Headers = new HttpListenerResponseHeaders();

            this.client = client;

            //Request = request;

            OutputStream = new MemoryStream();

            Version = request.Version;
            StatusCode = 200;
            ReasonPhrase = "OK";
        }

        //public HttpListenerRequest Request { get; private set; }

        public HttpListenerResponseHeaders Headers { get; private set; }

        public Stream OutputStream { get; private set; }

        public string Version { get; set; }

        public int StatusCode { get; set; }

        public string ReasonPhrase { get; set; }

        public long ContentLength
        {
            get
            {
                return this.OutputStream.Length;
            }
        }

        private async Task SendMessage()
        {
            var outputStream = OutputStream as MemoryStream;
            outputStream.Seek(0, SeekOrigin.Begin);

            var socketStream = client.GetOutputStream();

            string header = $"{Version} {StatusCode} {ReasonPhrase}\r\n" +
                            Headers.ToString() +
                            $"Content-Length: {outputStream.Length}\r\n" +
                            "\r\n";

            byte[] headerArray = Encoding.UTF8.GetBytes(header);
            await socketStream.WriteAsync(headerArray, 0, headerArray.Length);
            await outputStream.CopyToAsync(socketStream);

            await socketStream.FlushAsync();

        }

        public Task WriteAsync(string text)
        {
            var buffer = Encoding.UTF8.GetBytes(text);
            return OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        public async void Close()
        {
            await SendMessage();
            client.Dispose();
        }

        public async Task Redirect(Uri redirectLocation)
        {
            var outputStream = client.GetOutputStream();

            StatusCode = 301;
            ReasonPhrase = "Moved permanently";
            Headers.Location = redirectLocation;

            string header = $"{Version} {StatusCode} {ReasonPhrase}\r\n" +
                            $"Location: {Headers.Location}" +
                            $"Content-Length: 0\r\n" +
                            "Connection: close\r\n" +
                            "\r\n";

            byte[] headerArray = Encoding.UTF8.GetBytes(header);
            await outputStream.WriteAsync(headerArray, 0, headerArray.Length);
            await outputStream.FlushAsync();

        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                Close();

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~HttpListenerResponse() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}