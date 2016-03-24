#if WINDOWS_UWP || UAP10_0

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking;
using Windows.Networking.Sockets;

namespace System.Net.Http.Abstractions
{
    partial class TcpListenerAdapter
    {
        private StreamSocketListener _listener;

        private void Initialize()
        {
            _listener = new StreamSocketListener();
        }

        private Task<TcpClientAdapter> acceptTcpClientAsyncInternal() {
            var cts = new TaskCompletionSource<TcpClientAdapter>();
            TypedEventHandler<StreamSocketListener, StreamSocketListenerConnectionReceivedEventArgs> handler = null;
            handler = (sender, e) =>
            {
                var client = new TcpClientAdapter(e.Socket);
                cts.SetResult(client);
                _listener.ConnectionReceived -= handler;
            };
            _listener.ConnectionReceived += handler;
            return cts.Task;
        }

        public Task StartAsync() 
        {
            return _listener.BindEndpointAsync(
                new HostName(LocalEndpoint.Address.ToString()), LocalEndpoint.Port.ToString()).AsTask();
        }

        public void Stop() 
        {
            _listener.Dispose();
        }

        public StreamSocketListener StreamSocketListener
        {
            get
            {
                return this._listener;
            }
        }

    }

    partial class TcpClientAdapter
    {
        private StreamSocket tcpClient;

        public TcpClientAdapter(StreamSocket tcpClient)
        {
            this.tcpClient = tcpClient;

            LocalEndPoint = new IPEndPoint(
                IPAddress.Parse(tcpClient.Information.LocalAddress.ToString()),
                int.Parse(tcpClient.Information.LocalPort));

            RemoteEndPoint = new IPEndPoint(
                IPAddress.Parse(tcpClient.Information.RemoteAddress.ToString()),
                int.Parse(tcpClient.Information.RemotePort));
        }

        public Stream GetInputStream()
        {
            return this.tcpClient.InputStream.AsStreamForRead();
        }

        public Stream GetOutputStream()
        {
            return this.tcpClient.OutputStream.AsStreamForWrite();
        }

        public void Dispose()
        {
            this.tcpClient.Dispose();
        }
    }
}

#endif