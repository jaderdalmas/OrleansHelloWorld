using GrainInterfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

    public Task StartAsync(CancellationToken cancellationToken)
    {
      var grain = _client.GetGrain<IPrime>(0);
      var key = grain.GetGrainIdentity().PrimaryKey;

      //await grain.Consume();
      var stream = _client.GetStreamProvider(AppConst.SMSProvider)
        .GetStream<int>(key, AppConst.PSPrime);
      //await stream.SubscribeAsync(OnNextAsync);

      for (int mil = 0; mil < 1; mil++)
      {
        var tasks = new List<Task>();

        for (int dez = mil == 0 ? 100 : 0; dez < 1000; dez += 10)
        {
          var item = mil * 1000 + dez;

          tasks.Add(stream.OnNextAsync(item + 1));
          tasks.Add(stream.OnNextAsync(item + 3));
          tasks.Add(stream.OnNextAsync(item + 7));
          tasks.Add(stream.OnNextAsync(item + 9));
        }

        Task.WaitAll(tasks.ToArray());
      }

      return Task.CompletedTask;
    }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
    private Task OnNextAsync(int item, StreamSequenceToken token = null)
    {
      _logger.LogInformation($"OnNextAsync: item: {item}, token = {token}");
      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
  }
}
