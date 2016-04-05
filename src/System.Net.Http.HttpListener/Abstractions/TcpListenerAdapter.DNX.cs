#if DNXCORE50

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace System.Net.Http.Abstractions
{
    partial class TcpListenerAdapter
    {
        private TcpListener _tcpListener;

        private void Initialize()
        {
            _tcpListener = new TcpListener(LocalEndpoint);
        }

        private async Task<TcpClientAdapter> acceptTcpClientAsyncInternal() {
            var tcpClient = await _tcpListener.AcceptTcpClientAsync();
            return new TcpClientAdapter(tcpClient);
        }

        public void Start()
        {
            _tcpListener.Start();
        }

        public void Stop()
        {
            _tcpListener.Stop();
        }

        public Socket Socket
        {
            get
            {
                return _tcpListener.Server;
            }
        }

    }

    partial class TcpClientAdapter
    {
        private TcpClient tcpClient;

        public TcpClientAdapter(TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;

            LocalEndPoint = (IPEndPoint)tcpClient.Client.LocalEndPoint;
            RemoteEndPoint = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
        }

        public Stream GetInputStream()
        {
            return this.tcpClient.GetStream();
        }

        public Stream GetOutputStream()
        {
            return this.tcpClient.GetStream();
        }

        public void Dispose()
        {
            this.tcpClient.Dispose();
        }
    }
}

#endif