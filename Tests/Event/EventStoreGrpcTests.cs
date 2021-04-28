using EventStore.Client;

namespace Tests.Event
{
  public partial class EventStoreGrpcTests
  {
    private EventStoreClient GetCnn()
    {
      var settings = EventStoreClientSettings
        .Create("esdb://localhost:2113?tls=false");
      return new EventStoreClient(settings);
    }

    private string EventType => "TestEvent";
    private string TestStream => "test-stream";
    private string TestNoStream => "test-nostream";
  }
}
