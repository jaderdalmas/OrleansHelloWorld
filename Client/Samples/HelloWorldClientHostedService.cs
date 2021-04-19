using GrainInterfaces;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Runtime;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
  public class HelloWorldClientHostedService : IHostedService
  {
    private readonly IClusterClient _client;

    public HelloWorldClientHostedService(IClusterClient client)
    {
      _client = client;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
      // IHello
      var friend = _client.GetGrain<IHello>(0);
      var response = await friend.SayHello($"Good morning!");
      Console.WriteLine($"\n\n{response}\n\n");

      //IHelloArchive
      var g = _client.GetGrain<IHelloArchive>(0);
      response = await g.SayHello("Good day!");
      Console.WriteLine($"{response}");

      response = await g.SayHello("Good bye!");
      Console.WriteLine($"{response}");

      var greetings = await g.GetGreetings();
      Console.WriteLine($"\nArchived greetings: {Utils.EnumerableToString(greetings)}");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
  }
}
