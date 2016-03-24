using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.Net.Http
{
    public class HttpListenerRequestEventArgs : EventArgs
    {
        internal HttpListenerRequestEventArgs(HttpListenerRequest request, HttpListenerResponse response)
        {
            Request = request;
            Response = response;
        }

        public HttpListenerRequest Request { get; private set; }

        public HttpListenerResponse Response { get; private set; }
    }
}
