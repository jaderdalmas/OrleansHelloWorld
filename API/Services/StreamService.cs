using GrainInterfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace API.Services
{
  public class StreamService : IHostedService
  {
    private readonly ILogger<StreamService> _logger;
    private readonly IClusterClient _client;

    public StreamService(ILogger<StreamService> logger, IClusterClient client)
    {
      _logger = logger;
      _client = client;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
      var grain = _client.GetGrain<IHello>(0);
      var key = grain.GetGrainIdentity().PrimaryKey;

      var response = await grain.SayHello("Testing");
      Console.WriteLine($"{response}");

      var stream = _client.GetStreamProvider(AppConst.SMSProvider)
        .GetStream<string>(key, AppConst.PSHello);
      await stream.SubscribeAsync(OnNextAsync);

      for (int i = 1; i < 10; i++)
        await stream.OnNextAsync($"Good morning, {i}!");
    }

    private Task OnNextAsync(string item, StreamSequenceToken? token = null)
    {
      _logger.LogInformation($"OnNextAsync: item: {item}, token = {token}");
      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
  }
}
