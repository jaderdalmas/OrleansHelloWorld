using GrainInterfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using System.Collections.Generic;
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
      var grain = _client.GetGrain<IPrime>(0);
      var key = grain.GetGrainIdentity().PrimaryKey;

      var stream = _client.GetStreamProvider(AppConst.SMSProvider)
        .GetStream<int>(key, AppConst.PSPrime);
      await stream.SubscribeAsync(OnNextAsync);

      var tasks = new List<Task>();
      for (int i = 101; i < 110; i++)
      {
        tasks.Add(stream.OnNextAsync(i));

        if (i % 100 == 0)
          Task.WaitAll(tasks.ToArray());
      }
      Task.WaitAll(tasks.ToArray());
    }

    private Task OnNextAsync(int item, StreamSequenceToken token = null)
    {
      _logger.LogInformation($"OnNextAsync: item: {item}, token = {token}");
      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
  }
}
