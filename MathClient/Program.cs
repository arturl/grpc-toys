using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Utils;
using Math;

namespace MathClient
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Channel channel = new Channel("127.0.0.1:30051", ChannelCredentials.Insecure);

            var client = new MathService.MathServiceClient(channel);

            // Sync call:
            var reply = client.AddNumbers(new NumberTuple { N1 = 1, N2 = 2 });
            Console.WriteLine("(1) Added numbers: " + reply.N);

            // Async call
            reply = await client.AddNumbersAsync(new NumberTuple { N1 = 1, N2 = 2 });
            Console.WriteLine("(2) Added numbers: " + reply.N);

            // Server streaming
            var streamingCall = client.GenerateNumbers(new Number { N = 100 });

            Console.WriteLine("Reading from stream...");

            // await streamingCall.ResponseStream.ForEachAsync(async s => Console.WriteLine(s.N));

            while (await streamingCall.ResponseStream.MoveNext())
            {
                int n = streamingCall.ResponseStream.Current.N;
                Console.WriteLine("(3) Got number: " + n);
            }

            // Client streaming

            // TODO: show client pushing numbers to the server

            // Bi-directional streaming

            Console.WriteLine("Starting background task to receive messages");
            var bdStream = client.BiDirectionalStream();
            var readTask = Task.Run(async () =>
            {
                while (await bdStream.ResponseStream.MoveNext())
                {
                    int n = bdStream.ResponseStream.Current.N;
                    Console.WriteLine($"Received from server: {n}");
                }
            });

            Console.WriteLine("Starting to send messages");
            for(int i=100; i<105; i++)
            {
                Console.WriteLine($"Sending {i} to server");
                await bdStream.RequestStream.WriteAsync(new Number { N = i });
            }

            Console.WriteLine("Disconnecting");
            await bdStream.RequestStream.CompleteAsync();
            await readTask;

            channel.ShutdownAsync().Wait();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
