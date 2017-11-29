using System;
using System.Net;
using System.Threading.Tasks;

namespace SimpleHttpServer
{
    internal class Response
    {
        public string Body { get; set; }
        public int StatusCode { get; set; } = 200;
        public string ContentType { get; set; } = "application/json";
    }

    internal class SimpleHttpServer
    {
        readonly HttpListener _listener;
        private readonly Func<HttpListenerRequest, Response> _handle;
        private readonly string[] _prefixes;

        public SimpleHttpServer(Func<HttpListenerRequest, Response> handle, params string[] prefixes)
        {
            _listener = new HttpListener();
            if (!HttpListener.IsSupported)
            {
                throw new NotSupportedException("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
            }
            
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            _handle = handle;
            _prefixes = prefixes;
        }

        Task _runner;

        public void Start()
        {
            foreach (string s in _prefixes)
            {
                _listener.Prefixes.Add(s);
            }
            _listener.Start();

            _runner = Task.Run(async () =>
            {
                while (_listener.IsListening)
                {
                    HttpListenerContext context = await _listener.GetContextAsync();
                    Handle(context);
                }

            });
        }

        private void Handle(HttpListenerContext context)
        {
            Task.Run(async () =>
            {
                try
                {
                    var resp = _handle(context.Request);
                    byte[] buf = System.Text.Encoding.UTF8.GetBytes(resp.Body);
                    context.Response.ContentLength64 = buf.Length;
                    context.Response.StatusCode = resp.StatusCode;
                    context.Response.ContentType = resp.ContentType;
                    await context.Response.OutputStream.WriteAsync(buf, 0, buf.Length);
                }
                catch { } // suppress any exceptions
                finally
                {
                    // always close the stream
                    context.Response.OutputStream.Close();
                }

            });
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
            _runner?.Wait();
        }
    }
}
