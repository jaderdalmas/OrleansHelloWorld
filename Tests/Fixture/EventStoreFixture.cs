using EventStore.Client;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
  public class EventStoreFixture : IAsyncLifetime
  {
    public EventStoreFixture()
    {
      var settings = EventStoreClientSettings
        .Create("esdb://localhost:2113?tls=false");

      Client = new EventStoreClient(settings);
      PersistentSubscription = new EventStorePersistentSubscriptionsClient(settings);
    }

    public Task InitializeAsync()
    {
      return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
      await Client.DisposeAsync();
      await PersistentSubscription.DisposeAsync();
    }

    public EventStoreClient Client { get; private set; }
    public EventStorePersistentSubscriptionsClient PersistentSubscription { get; private set; }
  }
}
