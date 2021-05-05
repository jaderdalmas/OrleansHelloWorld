using EventStore.Client;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Event
{
  public partial class PersistSubsGrpcTests
  {
    private class TestEvent
    {
      public TestEvent() { }
      public TestEvent(string text)
      {
        EntityId = Guid.NewGuid().ToString("N");
        ImportantData = text;
      }

      public string EntityId { get; set; }
      public string ImportantData { get; set; }
    }

    [Fact]
    public async Task Test()
    {
      // Arrange
      await _persist.CreateAsync(TestStream, TestGroup, Settings);
      // Act
      await _persist.SubscribeAsync(TestStream, TestGroup, SubscribeReturn);

      var evt = new TestEvent("I wrote my first event!");
      var eventData = new EventData(
        Uuid.NewUuid(),
        evt.GetType().ToString(),
        JsonSerializer.SerializeToUtf8Bytes(evt)
      );
      await _client.AppendToStreamAsync(
        TestStream,
        StreamState.StreamExists,
        new[] { eventData }
      );

      await Task.Delay(TimeSpan.FromSeconds(10));

      // Clean
      await _persist.DeleteAsync(TestStream, TestGroup);
    }

    private async Task SubscribeReturn(PersistentSubscription ps, ResolvedEvent vnt, int? value, CancellationToken ct)
    {
      var json = Encoding.UTF8.GetString(vnt.Event.Data.Span);
      var result = JsonSerializer.Deserialize<TestEvent>(json);
      _ = result.ImportantData;

      await ps.Ack(new[] { vnt.Event.EventId });
    }
  }
}
