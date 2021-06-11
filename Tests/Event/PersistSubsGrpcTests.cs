using EventStore.Client;
using Xunit;

namespace Tests.Event
{
  [Collection(nameof(EventStoreCollection))]
  public partial class PersistSubsGrpcTests
  {
    private readonly EventStoreClient _client;
    private readonly EventStorePersistentSubscriptionsClient _persist;
    public PersistSubsGrpcTests(EventStoreFixture fixture)
    {
      _client = fixture.Client;
      _persist = fixture.PersistentSubscription;
    }

    private PersistentSubscriptionSettings Settings => new PersistentSubscriptionSettings();

    private string EventType => "TestEvent";
    private string TestStream => "test-stream";
    private string TestGroup => "test-group";
    private string TestNoStream => "test-nostream";
  }
}
