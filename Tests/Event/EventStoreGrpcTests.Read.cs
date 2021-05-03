using EventStore.Client;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Event
{
  public partial class EventStoreGrpcTests
  {
    [Fact]
    public async Task Read_NoStream()
    {
      var result = _client.ReadStreamAsync(Direction.Forwards, "hehehe", StreamPosition.Start);
      Assert.Equal(ReadState.StreamNotFound, await result.ReadState);
    }

    [Fact]
    public async Task Read_Stream()
    {
      var result = _client.ReadStreamAsync(Direction.Forwards, TestStream, StreamPosition.Start);
      Assert.NotEqual(ReadState.StreamNotFound, await result.ReadState);
      Assert.NotEmpty(result.ToEnumerable());
      Assert.Empty(result.ToEnumerable());

      result = _client.ReadStreamAsync(Direction.Forwards, TestStream, StreamPosition.Start);
      Assert.True(await result.AnyAsync());
      Assert.Empty(result.ToEnumerable());
    }

    [Fact]
    public async Task Read_All()
    {
      var result = _client.ReadAllAsync(Direction.Forwards, Position.Start, 100);

      if (await result.AnyAsync() == false)
        return;

      foreach (var vnt in result.ToEnumerable())
        Console.WriteLine(Encoding.UTF8.GetString(vnt.Event.Data.Span));
    }
  }
}
