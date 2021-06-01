using EventStore;
using EventStore.Client;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Event
{
  public partial class EventStoreGrpcTests
  {
    [Fact]
    public async Task Delete_Every()
    {
      // Arrange
      var streamName = $"{TestStream}-*";

      var list = await _client.GetStreams(streamName.Replace("*", ""));
      // Act
      foreach (var item in list)
        _ = await _client.SoftDeleteAsync(streamName, StreamState.Any);
      // Assert
      Assert.NotEmpty(list);
    }

    [Fact]
    public async Task Delete_State()
    {
      // Arrange
      var streamName = $"{TestStream}-01";

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
      var add = await _client.AppendToStreamAsync(
        streamName,
        StreamState.Any,
        new[] { eventData }
      );
      // Act
      var del = await _client.SoftDeleteAsync(streamName, StreamState.Any);
      // Assert
      Assert.NotEqual(Position.Start, del.LogPosition);
    }

    [Fact]
    public async Task Delete_State_Wrong()
    {
      // Arrange
      var streamName = $"{TestStream}-02";

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
      var add = await _client.AppendToStreamAsync(
        streamName,
        StreamState.Any,
        new[] { eventData }
      );

      try
      {
        // Act
        _ = await _client.SoftDeleteAsync(streamName, StreamState.NoStream);
      }
      catch (Exception ex)
      {
        // Assert
        Assert.IsType<WrongExpectedVersionException>(ex);
      }
    }

    [Fact]
    public async Task Delete_State_NoStream()
    {
      // Arrange
      var streamName = $"{TestStream}-None";
      try
      {
        // Act
        _ = await _client.SoftDeleteAsync(streamName, StreamState.StreamExists);
      }
      catch (Exception ex)
      {
        // Assert
        Assert.IsType<InvalidOperationException>(ex);
      }
    }

    [Fact]
    public async Task Delete_Revision()
    {
      // Arrange
      var streamName = $"{TestStream}-11";

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
      var add = await _client.AppendToStreamAsync(
        streamName,
        StreamState.Any,
        new[] { eventData }
      );
      // Act
      var del = await _client.SoftDeleteAsync(streamName, add.NextExpectedStreamRevision);
      // Assert
      Assert.NotEqual(Position.Start, del.LogPosition);
    }

    [Fact]
    public async Task Delete_Revision_Wrong()
    {
      // Arrange
      var streamName = $"{TestStream}-12";

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
      var add = await _client.AppendToStreamAsync(
        streamName,
        StreamState.Any,
        new[] { eventData }
      );

      try
      {
        // Act
        _ = await _client.SoftDeleteAsync(streamName, StreamRevision.None);
      }
      catch (Exception ex)
      {
        // Assert
        Assert.IsType<WrongExpectedVersionException>(ex);
      }
    }

    [Fact]
    public async Task Delete_Revision_NoStream()
    {
      // Arrange
      var streamName = $"{TestStream}-None";
      // Act
      var del = await _client.SoftDeleteAsync(streamName, StreamRevision.None);
      // Assert
      Assert.NotEqual(Position.Start, del.LogPosition);
    }
  }
}
