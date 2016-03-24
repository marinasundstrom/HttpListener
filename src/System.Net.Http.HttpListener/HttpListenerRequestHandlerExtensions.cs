using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.Net.Http
{
    public delegate Task HttpListenerRequestHandler(HttpListenerRequest request, HttpListenerResponse response);

    public static class HttpListenerRequestHandlerExtensions
    {
        public static object HttpMethod { get; private set; }

        public static void Get(this HttpListener httpListener, HttpListenerRequestHandler handler)
        {
            RegisterHandler(httpListener, handler, HttpMethods.Get, null);
        }

        public static void Get(this HttpListener httpListener, Uri uri, HttpListenerRequestHandler handler)
        {
            RegisterHandler(httpListener, handler, HttpMethods.Get, uri);
        }

        public static void Post(this HttpListener httpListener, HttpListenerRequestHandler handler)
        {
            RegisterHandler(httpListener, handler, HttpMethods.Post, null);
        }

        public static void Post(this HttpListener httpListener, Uri uri, HttpListenerRequestHandler handler)
        {
            RegisterHandler(httpListener, handler, HttpMethods.Post, uri);
        }

        public static void Put(this HttpListener httpListener, HttpListenerRequestHandler handler)
        {
            RegisterHandler(httpListener, handler, HttpMethods.Put, null);
        }

        public static void Put(this HttpListener httpListener, Uri uri, HttpListenerRequestHandler handler)
        {
            RegisterHandler(httpListener, handler, HttpMethods.Put, uri);
        }

        public static void Patch(this HttpListener httpListener, HttpListenerRequestHandler handler)
        {
            RegisterHandler(httpListener, handler, HttpMethods.Patch, null);
        }

        public static void Patch(this HttpListener httpListener, Uri uri, HttpListenerRequestHandler handler)
        {
            RegisterHandler(httpListener, handler, HttpMethods.Patch, uri);
        }

        public static void Delete(this HttpListener httpListener, HttpListenerRequestHandler handler)
        {
            RegisterHandler(httpListener, handler, HttpMethods.Delete, null);
        }

        public static void Delete(this HttpListener httpListener, Uri uri, HttpListenerRequestHandler handler)
        {
            RegisterHandler(httpListener, handler, HttpMethods.Delete, uri);
        }

        private static void RegisterHandler(HttpListener httpListener, HttpListenerRequestHandler handler, string method, Uri url)
        {
            httpListener.Request += (sender, context) =>
            {
                var request = context.Request;
                var response = context.Response;

                bool proceed = true;
                if (url != null)
                {
                    if (url.IsAbsoluteUri)
                    {
                        proceed = Uri.Compare(url, request.Url, UriComponents.AbsoluteUri, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase) == 0;
                    }
                    else
                    {
                        proceed = request.Url.LocalPath.Contains(url.OriginalString);
                    }
                }
                if (proceed)
                {
                    TryCallHandler(handler, context, method);
                }
                else
                {
                    response.NotFound();
                    response.Close();
                }
            };
        }

        private static async void TryCallHandler(HttpListenerRequestHandler handler, HttpListenerRequestEventArgs context, string requestHttpMethod)
        {
            var request = context.Request;
            var response = context.Response;

            if (request.HttpMethod == requestHttpMethod)
            {
                await handler(request, response);

                response.Close();
            }
        }
    }
}