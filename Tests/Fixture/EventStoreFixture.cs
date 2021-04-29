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
    }

    public Task InitializeAsync()
    {
      return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
      await Client.DisposeAsync();
    }

    public EventStoreClient Client { get; private set; }
  }
}
