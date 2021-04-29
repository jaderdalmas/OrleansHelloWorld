using EventStore.Client;
using Xunit;

namespace Tests.Event
{
  [Collection(nameof(EventStoreCollection))]
  public partial class EventStoreGrpcTests
  {
    private readonly EventStoreClient _client;
    public EventStoreGrpcTests(EventStoreFixture fixture)
    {
      _client = fixture.Client;
    }

    private string EventType => "TestEvent";
    private string TestStream => "test-stream";
    private string TestNoStream => "test-nostream";
  }
}
