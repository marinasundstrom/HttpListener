using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Http
{
    public class HttpListenerResponse : IDisposable
    {
        private TcpClient client;

        internal HttpListenerResponse(HttpListenerRequest request, TcpClient client)
        {
            Headers = new Dictionary<string, object>();

            this.client = client;

            Request = request;

            OutputStream = new MemoryStream();

            ProtocolVersion = request.ProtocolVersion;
            StatusCode = 200;
            StatusDescription = "OK";
        }

        public HttpListenerRequest Request { get; private set; }

        public IDictionary<string, object> Headers { get; private set; }

        public Stream OutputStream { get; private set; }

        public string ProtocolVersion { get; private set; }

        public int StatusCode { get; set; }

        public string StatusDescription { get; set; }

        public bool KeepAlive
        {
            get
            {
                return Headers?["Connection"].ToString() == "keep-alive";
            }

            set
            {
                if (value)
                {
                    Headers["Connection"] = "keep-alive";

                }
                else
                {
                    Headers["Connection"] = "close";
                }
            }
        }

        public string ContentType
        {
            get
            {
                return Headers?["Content-Type"].ToString();
            }

            set
            {
                Headers["Content-Type"] = value;
            }
        }

        public Uri RedirectLocation
        {
            get
            {
                return new Uri(Headers?["Location"].ToString());
            }

            set
            {
                Headers["Location"] = value.ToString();
            }
        }

        public long ContentLength
        {
            get
            {
                return this.OutputStream.Length;
            }
        }

        private string MakeHeaders()
        {
            var sb = new StringBuilder();
            foreach (var header in Headers)
            {
                sb.Append($"{header.Key}: {header.Value}\r\n");
            }
            return sb.ToString();
        }

        private async Task SendMessage()
        {
            var outputStream = OutputStream as MemoryStream;
            outputStream.Seek(0, SeekOrigin.Begin);

            var socketStream = client.GetStream();

            string header = $"{ProtocolVersion} {StatusCode} {StatusDescription}\r\n" +
                            MakeHeaders() +
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

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
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