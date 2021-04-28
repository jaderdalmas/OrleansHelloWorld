using EventStore.Client;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Event
{
  public partial class EventStoreGrpcTests
  {
    [Fact]
    public async Task Read_Stream()
    {
      var client = GetCnn();

      var data = new
      {
        EntityId = Guid.NewGuid().ToString("N"),
        ImportantData = "I wrote my first event!"
      };

      var eventData = new EventData(
        Uuid.NewUuid(),
        EventType,
        JsonSerializer.SerializeToUtf8Bytes(data)
      );

      await client.AppendToStreamAsync(
        TestStream,
        StreamState.Any,
        new[] { eventData }
      );

      var result = client.ReadStreamAsync(
        Direction.Forwards,
        TestStream,
        StreamPosition.Start
      );

      if (await result.ReadState == ReadState.StreamNotFound)
        return;

      foreach (var vnt in result.ToEnumerable())
        Console.WriteLine(Encoding.UTF8.GetString(vnt.Event.Data.Span));
    }

    [Fact]
    public Task Read_All()
    {
      var client = GetCnn();

      var result = client.ReadAllAsync(Direction.Forwards, Position.Start);

      foreach (var vnt in result.ToEnumerable())
        Console.WriteLine(Encoding.UTF8.GetString(vnt.Event.Data.Span));

      return Task.CompletedTask;
    }
  }
}
