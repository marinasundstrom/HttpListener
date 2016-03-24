using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Abstractions;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Http
{
    public class HttpListenerRequest
    {
        private TcpClientAdapter client;

        internal HttpListenerRequest()
        {
            Headers = new Dictionary<string, object>();
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

            var headers = ParseHeaders(lines);
            foreach (var header in headers)
            {
                Headers[header.Key] = header.Value;
            }

            var url = (new UriBuilder(headers["Host"].ToString() + requestParts[1])).Uri;
            var httpMethod = requestParts[0];

            ProtocolVersion = requestParts[2];
            HttpMethod = httpMethod;
            Url = url;

            if (HttpMethod == HttpMethods.Post || HttpMethod == HttpMethods.Put || HttpMethod == HttpMethods.Patch)
            {
                var encoding = Encoding.UTF8; // GetEncoding(ContentType);

                char[] buffer = new char[ContentLength];

                await reader.ReadAsync(buffer, 0, ContentLength);

                InputStream = new MemoryStream(encoding.GetBytes(buffer));
            }
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

        IDictionary<string, object> ParseHeaders(IEnumerable<string> lines)
        {

            var headers = new Dictionary<string, object>();

            foreach (var headerLine in lines.Skip(1))
            {
                var parts = headerLine.Split(':');
                var key = parts[0];
                var value = parts[1].Trim();
                headers.Add(key, value);
            }

            return headers;
        }


        public IPEndPoint LocalEndpoint { get; private set; }

        public IPEndPoint RemoteEndpoint { get; private set; }

        public Uri Url { get; private set; }

        public string HttpMethod { get; private set; }

        public IDictionary<string, object> Headers { get; private set; }

        public Stream InputStream { get; private set; }

        public string ProtocolVersion { get; private set; }

        public string Host
        {
            get
            {
                return Headers?["Host"].ToString();
            }
        }

        public bool KeepAlive
        {
            get
            {
                return Headers?["Connection"].ToString() == "keep-alive";
            }
        }

        public string UserAgent
        {
            get
            {
                return Headers?["User-Agent"].ToString();
            }
        }

        public string Accept
        {
            get
            {
                return Headers?["Accept"].ToString();
            }
        }

        public string AcceptEncoding
        {
            get
            {
                return Headers?["Accept-Encoding"].ToString();
            }
        }

        public string AcceptLanguage
        {
            get
            {
                return Headers?["Accept-Language"].ToString();
            }
        }

        public string ContentType
        {
            get
            {
                return Headers?["Content-Type"].ToString();
            }
        }

        public int ContentLength
        {
            get
            {
                return int.Parse(Headers?["Content-Length"].ToString());
            }
        }
    }
}