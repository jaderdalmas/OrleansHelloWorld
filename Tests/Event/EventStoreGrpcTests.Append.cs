using EventStore.Client;
using System;
using System.Collections.Generic;
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
    public async Task Append()
    {
      var evt = new
      {
        EntityId = Guid.NewGuid().ToString("N"),
        ImportantData = "I wrote my first event!"
      };

      var eventData = new EventData(
        Uuid.NewUuid(),
        EventType,
        JsonSerializer.SerializeToUtf8Bytes(evt)
      );

      await _client.AppendToStreamAsync(
        TestStream,
        StreamState.Any,
        new[] { eventData }
      );
    }

    [Fact]
    public async Task Append_NoStream()
    {
      var evt = "{\"id\": \"1\" \"value\": \"some value\"}";
      var eventData = new EventData(
        Uuid.NewUuid(),
        EventType,
        Encoding.UTF8.GetBytes(evt)
      );

      try
      {
        await _client.AppendToStreamAsync(
          TestNoStream,
          StreamState.NoStream,
          new EventData[] { eventData }
        );
      }
      catch (Exception ex)
      {
        Assert.IsType<WrongExpectedVersionException>(ex);
      }
    }

    [Fact]
    public async Task Append_Only1()
    {
      var evt = "{\"id\": \"1\" \"value\": \"some value\"}";
      var eventData = new EventData(
        Uuid.NewUuid(),
        EventType,
        Encoding.UTF8.GetBytes(evt)
      );

      await _client.AppendToStreamAsync(
        TestStream,
        StreamState.Any,
        new EventData[] { eventData }
      );

      await _client.AppendToStreamAsync(
        TestStream,
        StreamState.Any,
        new EventData[] { eventData }
      );
    }

    [Fact]
    public async Task Append_Concurrency()
    {
      var clientOneRead = _client.ReadStreamAsync(
        Direction.Forwards,
        TestNoStream,
        StreamPosition.Start,
        configureOperationOptions: options => options.ThrowOnAppendFailure = false);
      var clientOneRevision = (await clientOneRead.LastAsync()).Event.EventNumber.ToUInt64();

      var clientTwoRead = _client.ReadStreamAsync(Direction.Forwards, "some-nostream", StreamPosition.Start);
      var clientTwoRevision = (await clientTwoRead.LastAsync()).Event.EventNumber.ToUInt64();

      var clientOneData = new EventData(
        Uuid.NewUuid(),
        EventType,
        Encoding.UTF8.GetBytes("{\"id\": \"1\" \"value\": \"clientOne\"}")
      );

      await _client.AppendToStreamAsync(
        TestNoStream,
        clientOneRevision,
        new List<EventData> {
          clientOneData
        });

      var clientTwoData = new EventData(
        Uuid.NewUuid(),
        EventType,
        Encoding.UTF8.GetBytes("{\"id\": \"2\" \"value\": \"clientTwo\"}")
      );

      try
      {
        await _client.AppendToStreamAsync(
          TestNoStream,
          clientTwoRevision,
          new List<EventData> {
          clientTwoData
          });
      }
      catch (Exception ex)
      {
        Assert.IsType<WrongExpectedVersionException>(ex);
      }
    }
  }
}
