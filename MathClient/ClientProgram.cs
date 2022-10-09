using System;
//using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using BotBuilderMock.Microsoft.Bot.Schema;
using Grpc.Core;
using Grpc.Core.Utils;
using Math;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;
using ProtoBuf;
using System.Diagnostics;
using Activity = BotBuilderMock.Microsoft.Bot.Schema.Activity;
using System.Collections;
using System.IO.IsolatedStorage;

namespace MathClient
{
    class ClientProgram
    {
        public static async Task Main(string[] args)
        {
            var files = Directory.EnumerateFiles("..\\..\\..\\..\\TestData\\Activities").ToArray();

            // Setup: read all activities from files
            var activities = files.Select(file => JsonConvert.DeserializeObject<Activity>(File.ReadAllText(file)));

            int totalSizeOfUtf = 0;
            var activitiesSerializedAsJSON = new List<byte[]>();

            // Serialize all activities into UTF8, calculate size for JSON vs. Protobuf
            foreach (var activity in activities)
            {
                var stringActivity = JsonConvert.SerializeObject(activity, Formatting.None);
                var bytes = Encoding.UTF8.GetBytes(stringActivity);

                totalSizeOfUtf += bytes.Length;
                activitiesSerializedAsJSON.Add(bytes);
            }

            Console.WriteLine($"Average size in UTF8 JSON: {totalSizeOfUtf / activities.Count()}");

            totalSizeOfUtf = 0;
            var activitiesSerializedAsProtobuf = new List<byte[]>();
            foreach (var activity in activities)
            {
                var bytes = ProtoSerialize(activity);

                totalSizeOfUtf += bytes.Length;
                activitiesSerializedAsProtobuf.Add(bytes);
            }
            Console.WriteLine($"Average size in UTF8 JSON: {totalSizeOfUtf / activities.Count()}");

            // Measure serialization time

            var measureSerializationJson = MeasureExecutionTime(() =>
            {
                int totalSizeLocal = 0;
                int Repeat = 100;
                for (int i = 0; i < Repeat; i++)
                {
                    foreach (var activity in activities)
                    {
                        var stringActivity = JsonConvert.SerializeObject(activity, Formatting.None);
                        var bytes = Encoding.UTF8.GetBytes(stringActivity);

                        totalSizeLocal += bytes.Length;
                    }
                }
                return totalSizeLocal;
            });
            Console.WriteLine($"Elapsed time for serialization into JSON is {measureSerializationJson.Item1} ms. Processed {measureSerializationJson.Item2} bytes");

            var measureSerializationProtobuf = MeasureExecutionTime(() =>
            {
                int totalSizeLocal = 0;
                int Repeat = 100;
                for (int i = 0; i < Repeat; i++)
                {
                    foreach (var activity in activities)
                    {
                        var bytes = ProtoSerialize(activity);
                        totalSizeLocal += bytes.Length;
                    }
                }
                return totalSizeLocal;
            });
            Console.WriteLine($"Elapsed time for serialization into Protobuf is {measureSerializationProtobuf.Item1} ms. Processed {measureSerializationProtobuf.Item2} bytes");

            // Measure deserialization time

            var measuredTime = MeasureExecutionTime(() =>
            {
                var allActivities = new List<Activity>();
                int Repeat = 1000;
                for (int i = 0; i < Repeat; i++)
                {
                    foreach (var byteArray in activitiesSerializedAsJSON)
                    {
                        var activity = JsonConvert.DeserializeObject<Activity>(Encoding.UTF8.GetString(byteArray));

                        // Prevent optimizer from optimizing away dead store
                        allActivities.Add(activity);
                    }
                }
                return allActivities.Count();
            });

            Console.WriteLine($"Elapsed time for deserialization from JSON is {measuredTime.Item1} ms. Processed {measuredTime.Item2} items");

            var measuredTime2 = MeasureExecutionTime(() =>
            {
                var allActivities = new List<Activity>();
                int Repeat = 1000;
                for (int i = 0; i < Repeat; i++)
                {
                    foreach (var byteArray in activitiesSerializedAsProtobuf)
                    {
                        var activity = ProtoDeserialize<Activity>(byteArray);

                        // Prevent optimizer from optimizing away dead store
                        allActivities.Add(activity);
                    }
                }
                return allActivities.Count();
            });

            Console.WriteLine($"Elapsed time for deserialization from Protobuf is {measuredTime2.Item1} ms. Processed {measuredTime2.Item2} items");
        }

        private static (long,long) MeasureExecutionTime(Func<long> f)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            long result = f();
            stopWatch.Stop();
            return (stopWatch.ElapsedMilliseconds, result);
        }

        private static byte[] ProtoSerialize<T>(T record)
        {
            using var stream = new MemoryStream();
            Serializer.Serialize(stream, record);
            return stream.ToArray();
        }

        private static Activity ProtoDeserialize<T>(byte[] bytes)
        {
            return Serializer.Deserialize<Activity>(new MemoryStream(bytes));
        }

        public static async Task Main2(string[] args)
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
