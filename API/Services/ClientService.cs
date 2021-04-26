using GrainInterfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Hosting;
using Orleans.Runtime;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace API.Services
{
  public class ClientService : IHostedService
  {
    private readonly ILogger<ClientService> _logger;

    public ClientService(ILogger<ClientService> logger, ILoggerProvider loggerProvider)
    {
      _logger = logger;
      Client = new ClientBuilder()
        .UseLocalhostClustering()
        //.Configure_Grains(new List<Assembly>() { Grains.AppConst.Assembly })
        .AddSimpleMessageStreamProvider(AppConst.SMSProvider)
        .ConfigureLogging(builder => builder.AddProvider(loggerProvider))
        .Build();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      var attempt = 0;
      var maxAttempts = 100;
      var delay = TimeSpan.FromSeconds(1);
      var message = "Failed to connect to Orleans cluster on attempt {@Attempt} of {@MaxAttempts}.";
      return Client.Connect(async error =>
      {
        if (cancellationToken.IsCancellationRequested)
          return false;

        if (attempt++ > maxAttempts)
        {
          _logger.LogError(error, message, attempt, maxAttempts);

          return false;
        }

        _logger.LogWarning(error, message, attempt, maxAttempts);
        try
        {
          await Task.Delay(delay, cancellationToken);
        }
        catch (OperationCanceledException)
        {
          return false;
        }

        return true;
      });
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
      try
      {
        await Client.Close();
      }
      catch (OrleansException error)
      {
        _logger.LogWarning(error, "Error while gracefully disconnecting from Orleans cluster. Will ignore and continue to shutdown.");
      }
    }

    public IClusterClient Client { get; }
  }

  public static class ClientServiceExtensions
  {
    public static IServiceCollection AddClientService(this IServiceCollection services)
    {
      services.AddSingleton<ClientService>();
      services.AddSingleton<IHostedService>(_ => _.GetService<ClientService>());
      services.AddSingleton(_ => _.GetService<ClientService>().Client);
      return services;
    }
  }
}
