// See https://aka.ms/new-console-template for more information
using Grpc.Core;
using Helloworld;

class MathImpl : Greeter.GreeterBase
{
    public override Task AddNumbers(NumberTuple request, IServerStreamWriter<Number> responseStream, ServerCallContext context)
    {
        return Task.FromResult(new Number { N = request.N1 + request.N2 });
    }
}

class Program
{
    const int Port = 30051;
    static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        Server server = new Server
        {
            Services = { Greeter.BindService(new MathImpl()) },
            Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
        };
        server.Start();

        Console.WriteLine("Greeter server listening on port " + Port);
        Console.WriteLine("Press any key to stop the server...");
        Console.ReadKey();

        server.ShutdownAsync().Wait();

    }
}




