using GrainInterfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
  public class HelloWorldClientHostedService : IHostedService
  {
    private readonly ILogger _logger;
    private readonly IClusterClient _client;

    public HelloWorldClientHostedService(ILogger<HelloWorldClientHostedService> logger, IClusterClient client)
    {
      _logger = logger;
      _client = client;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
      //await Simple();
      await Stream();
    }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Old Version")]
    private async Task Simple()
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

    public async Task Stream()
    {
      var grain = _client.GetGrain<IHello>(0);
      var key = grain.GetGrainIdentity().PrimaryKey;

      var response = await grain.SayHello("Testing");
      Console.WriteLine($"{response}");

      var stream = _client.GetStreamProvider(AppConst.SMSProvider)
        .GetStream<string>(key, AppConst.PSHello);
      //await stream.SubscribeAsync(OnNextAsync);

      for (int i = 1; i < 10; i++)
        await stream.OnNextAsync($"Good morning, {i}!");
    }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Testing")]
    private Task OnNextAsync(string item, StreamSequenceToken token = null)
    {
      _logger.LogInformation($"OnNextAsync: item: {item}, token = {token}");
      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
  }
}
