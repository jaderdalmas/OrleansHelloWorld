using Microsoft.Extensions.Hosting;
using Orleans;
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

    public Task StartAsync(CancellationToken cancellationToken)
    {
      HelloWorld.ManyHello(_client);

      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
  }
}
