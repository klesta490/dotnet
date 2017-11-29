using System.IO;
using System.Net;
using System.Threading;
using Newtonsoft.Json;

namespace SimpleHttpServer
{
    internal class Logic
    {
        private int _counter = 0;
        private readonly JsonSerializer _serializer = JsonSerializer.Create();

        public Response Handle(HttpListenerRequest request)
        {
            var res = Interlocked.Increment(ref _counter);

            using (var stringWriter = new StringWriter())
            using (var jsonWriter = new JsonTextWriter(stringWriter))
            {
                _serializer.Serialize(jsonWriter, new { count = res });

                return new Response() { Body = stringWriter.ToString() };
            }
        }
    }
}