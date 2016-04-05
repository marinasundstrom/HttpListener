using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Abstractions;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Http
{
    public sealed class HttpListenerRequest
    {
        private TcpClientAdapter client;

        internal HttpListenerRequest()
        {
            Headers = new HttpListenerRequestHeaders();
        }

        internal async Task ProcessAsync(TcpClientAdapter client)
        {
            this.client = client;

            var reader = new StreamReader(client.GetInputStream());

            StringBuilder request = await ReadRequest(reader);

            var localEndpoint = client.LocalEndPoint;
            var remoteEnpoint = client.RemoteEndPoint;


            // This code needs to be rewritten and simplified.

            var requestLines = request.ToString().Split('\n');
            string requestMethod = requestLines[0].TrimEnd('\r');
            string[] requestParts = requestMethod.Split(' ');

            LocalEndpoint = (System.Net.IPEndPoint)localEndpoint;
            RemoteEndpoint = (System.Net.IPEndPoint)remoteEnpoint;

            var lines = request.ToString().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            ParseHeaders(lines);
            ParseRequestLine(lines);

            await PrepareInputStream(reader);
        }

        private void ParseRequestLine(string[] lines)
        {
            var line = lines.ElementAt(0).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var url = new UriBuilder(Headers.Host + line[1]).Uri;
            var httpMethod = line[0];

            Version = line[2];
            Method = httpMethod;
            RequestUri = url;
        }

        private async Task PrepareInputStream(StreamReader reader)
        {
            if (Method == HttpMethods.Post || Method == HttpMethods.Put || Method == HttpMethods.Patch)
            {
                Encoding encoding = Encoding.UTF8;

                char[] buffer = new char[Headers.ContentLength];

                await reader.ReadAsync(buffer, 0, Headers.ContentLength);

                InputStream = new MemoryStream(encoding.GetBytes(buffer));
            }
        }

        private void ParseHeaders(IEnumerable<string> lines)
        {
            lines = lines.Skip(1);
            Headers.ParseHeaderLines(lines);
        }

        private static async Task<StringBuilder> ReadRequest(StreamReader reader)
        {
            var request = new StringBuilder();

            string line = null;
            while ((line = await reader.ReadLineAsync()) != "")
            {
                request.AppendLine(line);
            }

            var requestStr = request.ToString();
            return request;
        }

        public IPEndPoint LocalEndpoint { get; private set; }

        public IPEndPoint RemoteEndpoint { get; private set; }

        public Uri RequestUri { get; private set; }

        public string Method { get; private set; }

        public HttpListenerRequestHeaders Headers { get; private set; }

        public Stream InputStream { get; private set; }

        public string Version { get; private set; }  

        public bool IsLocal
        {
            get
            {
                return RemoteEndpoint.Address.Equals(LocalEndpoint.Address);
            }
        }
    }
}