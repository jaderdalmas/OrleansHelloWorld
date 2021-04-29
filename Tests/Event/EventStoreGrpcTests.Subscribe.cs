using EventStore.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Event
{
  public partial class EventStoreGrpcTests
  {
    private Task SubscribeReturn(StreamSubscription ss, ResolvedEvent vnt, CancellationToken ct)
    {
      Console.WriteLine($"Received event {vnt.OriginalEventNumber}@{vnt.OriginalStreamId}");
      Console.WriteLine($"Data {Encoding.UTF8.GetString(vnt.Event.Data.Span)}");
      return Task.CompletedTask;
    }

    [Fact]
    public async Task SubscribeStream()
    {
      await _client.SubscribeToStreamAsync(TestStream, SubscribeReturn);
    }

    [Fact]
    public async Task SubscribeAll()
    {
      await _client.SubscribeToAllAsync(SubscribeReturn);
    }

    private Task SubscribeFilterReturn(StreamSubscription ss, Position p, CancellationToken ct)
    {
      Console.WriteLine($"checkpoint taken at {p.PreparePosition}");
      return Task.CompletedTask;
    }

    [Fact]
    public async Task SubscribeAll_StreamFilterPrefix()
    {
      var prefixStreamFilter = new SubscriptionFilterOptions(StreamFilter.Prefix("test-", "other-"));
      await _client.SubscribeToAllAsync(SubscribeReturn,
        filterOptions: prefixStreamFilter);
    }

    [Fact]
    public async Task SubscribeAll_StreamFilterExpression()
    {
      var prefixStreamFilter = new SubscriptionFilterOptions(StreamFilter.RegularExpression("^test|^other"));
      await _client.SubscribeToAllAsync(SubscribeReturn,
        filterOptions: prefixStreamFilter);
    }

    [Fact]
    public async Task SubscribeAll_EventFilterSystem()
    {
      var typeEventFilter = new SubscriptionFilterOptions(EventTypeFilter.ExcludeSystemEvents(),
        checkpointInterval: 5, checkpointReached: SubscribeFilterReturn);
      await _client.SubscribeToAllAsync(SubscribeReturn,
        filterOptions: typeEventFilter);
    }

    [Fact]
    public async Task SubscribeAll_EventFilterPrefix()
    {
      var prefixEventFilter = new SubscriptionFilterOptions(EventTypeFilter.Prefix("test-"));
      await _client.SubscribeToAllAsync(SubscribeReturn,
        filterOptions: prefixEventFilter);
    }

    [Fact]
    public async Task SubscribeAll_EventFilterExpression()
    {
      var prefixEventFilter = new SubscriptionFilterOptions(EventTypeFilter.RegularExpression("^test"));
      await _client.SubscribeToAllAsync(SubscribeReturn,
        filterOptions: prefixEventFilter);
    }
  }
}
