using Xunit;

namespace Tests
{
  [CollectionDefinition(nameof(EventStoreCollection))]
  public class EventStoreCollection : ICollectionFixture<EventStoreFixture> { }
}
