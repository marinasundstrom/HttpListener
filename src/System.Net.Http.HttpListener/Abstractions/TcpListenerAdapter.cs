using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace System.Net.Http.Abstractions
{
    partial class TcpListenerAdapter
    {
        public TcpListenerAdapter(IPEndPoint localEndpoint)
        {
            LocalEndpoint = localEndpoint;

            Initialize();
        }

        public IPEndPoint LocalEndpoint { get; private set; }
       
        public Task<TcpClientAdapter> AcceptTcpClientAsync() 
        {
            return acceptTcpClientAsyncInternal();
        }
    }

    partial class TcpClientAdapter
    {
        public IPEndPoint LocalEndPoint
        {
            get;
            private set;
        }

        public IPEndPoint RemoteEndPoint
        {
            get;
            private set;
        }
    }
}