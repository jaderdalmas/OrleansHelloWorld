using Microsoft.Extensions.Hosting;
using Orleans;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
  public class PrimeClientHostedService : IHostedService
  {
    private readonly IClusterClient _client;

    public PrimeClientHostedService(IClusterClient client)
    {
      _client = client;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      Prime.ManyPrime(_client);

      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
  }
}
