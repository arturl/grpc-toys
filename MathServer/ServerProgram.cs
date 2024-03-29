﻿// See https://aka.ms/new-console-template for more information
using Grpc.Core;
using Math;
using Microsoft.Bot.Schema;

class MathImpl : MathService.MathServiceBase
{
    public override Task<Number> AddNumbers(NumberTuple request, ServerCallContext context)
    {
        return Task.FromResult(new Number { N = request.N1 + request.N2 });
    }

    public override async Task GenerateNumbers(Number request, IServerStreamWriter<Number> responseStream, ServerCallContext context)
    {
        for (int i = request.N; i < request.N + 5; i++)
        {
            Console.WriteLine($"Server sends {i}");
            await responseStream.WriteAsync(new Number { N = i });
        }
    }

    // Server sends data to the client via a bidirectional stream
    public override async Task BiDirectionalStream(IAsyncStreamReader<Number> requestStream, IServerStreamWriter<Number> responseStream, ServerCallContext context)
    {
        // Simulate server-initiated message, not a resonse to a client message
        var sendTask = Task.Run(async () =>
        {
            await Task.Delay(3000); // happens after a delay, to mimic a reaction to an external stimulus
            Console.WriteLine($"Server sends an out of band message");
            await responseStream.WriteAsync(new Number { N = 999 });
        });

        while (await requestStream.MoveNext())
        {
            int n = requestStream.Current.N;
            await responseStream.WriteAsync(new Number { N = n * 10 });
            await responseStream.WriteAsync(new Number { N = n * 100 });
        }

        await sendTask;
    }
}

class ServerProgram
{
    const int Port = 30051;
    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting the server");

        Activity? a = null;

        Server server = new Server
        {
            Services = { MathService.BindService(new MathImpl()) },
            Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
        };
        server.Start();

        Console.WriteLine("Greeter server listening on port " + Port);
        Console.WriteLine("Press any key to stop the server...");
        Console.ReadKey();

        server.ShutdownAsync().Wait();

    }
}




