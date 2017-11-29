using System;
using System.Threading.Tasks;

namespace SimpleHttpServer
{
    class Program
    {
        private static readonly TaskCompletionSource<bool> End = new TaskCompletionSource<bool>();

        static void Main(string[] args)
        {
            var logic = new Logic();
            var server = new SimpleHttpServer(logic.Handle, "http://localhost:80/");
            server.Start();

            Console.CancelKeyPress += ConsoleOnCancelKeyPress;
            End.Task.Wait();
        }

        static void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs consoleCancelEventArgs)
        {
            End.TrySetResult(true);
        }
    }
}
