using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EventStore
{
  public interface IEventStoreService
  {
    EventStoreClient Client { get; }
    EventStorePersistentSubscriptionsClient PersistentSubscription { get; }
  }

  public class EventStoreService : IEventStoreService
  {
    public EventStoreClient Client { get; }
    public EventStorePersistentSubscriptionsClient PersistentSubscription { get; }

    public EventStoreService(IOptions<EventStoreSettings> config)
    {
      var cnn = string.IsNullOrWhiteSpace(config?.Value?.Connection) ? "esdb://localhost:2113?tls=false" : config.Value.Connection;
      var settings = EventStoreClientSettings.Create(cnn);

      Client = new EventStoreClient(settings);
      PersistentSubscription = new EventStorePersistentSubscriptionsClient(settings);
    }
  }

  public static class EventStoreServiceExtensions
  {
    public static IServiceCollection AddEventStoreService(this IServiceCollection services)
    {
      services.AddSingleton<IEventStoreService, EventStoreService>();
      services.AddSingleton(_ => _.GetService<IEventStoreService>().Client);
      services.AddSingleton(_ => _.GetService<IEventStoreService>().PersistentSubscription);
      return services;
    }
  }
}
