using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using System;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Event
{
  public class EventStoreTcpTests
  {
    private IEventStoreConnection GetCnn()
    {
      return EventStoreConnection.Create(
        new Uri("tcp://admin:changeit@localhost:1113"));
    }

    private IEventStoreConnection LiveCnn()
    {
      var settingsBuilder = ConnectionSettings.Create();
      settingsBuilder.SetDefaultUserCredentials(
        new UserCredentials("admin", "changeit"));
      settingsBuilder.SetCompatibilityMode("auto");

      var settings = settingsBuilder.Build();

      var clusterSettings = ClusterSettings.Create()
        .DiscoverClusterViaDns()
        .SetClusterDns("localhost")
        .SetClusterGossipPort(2113);

      return EventStoreConnection.Create(settings, clusterSettings);
    }

    [Fact]
    public async Task Connect()
    {
      var cnn = GetCnn();

      await cnn.ConnectAsync();
    }

    [Fact]
    public async Task Append()
    {
      var cnn = GetCnn();
      await cnn.ConnectAsync();

      const string streamName = "newstream";
      const string eventType = "event-type";
      const string data = "{ \"a\":\"2\"}";
      const string metadata = "{}";

      var eventPayload = new EventData(
          eventId: Guid.NewGuid(),
          type: eventType,
          isJson: true,
          data: Encoding.UTF8.GetBytes(data),
          metadata: Encoding.UTF8.GetBytes(metadata)
      );
      await cnn.AppendToStreamAsync(streamName, ExpectedVersion.Any, eventPayload);
    }

    [Fact]
    public async Task Read()
    {
      var cnn = GetCnn();
      await cnn.ConnectAsync();

      const string streamName = "newstream";
      const string eventType = "event-type";
      const string data = "{ \"a\":\"2\"}";
      const string metadata = "{}";

      var eventPayload = new EventData(
          eventId: Guid.NewGuid(),
          type: eventType,
          isJson: true,
          data: Encoding.UTF8.GetBytes(data),
          metadata: Encoding.UTF8.GetBytes(metadata)
      );
      await cnn.AppendToStreamAsync(streamName, ExpectedVersion.Any, eventPayload);

      var readEvents = await cnn.ReadStreamEventsForwardAsync(streamName, 0, 10, true);
      foreach (var evt in readEvents.Events)
      {
        Console.WriteLine(Encoding.UTF8.GetString(evt.Event.Data));
      }
    }

    [Fact]
    public async Task Read1()
    {
      var cnn = GetCnn();
      await cnn.ConnectAsync();

      var data = Encoding.UTF8.GetBytes("{\"a\":\"2\"}");
      var metadata = Encoding.UTF8.GetBytes("{}");
      var evt = new EventData(Guid.NewGuid(), "testEvent", true, data, metadata);
      await cnn.AppendToStreamAsync("test-stream", ExpectedVersion.Any, evt);

      var streamEvents = await cnn.ReadStreamEventsForwardAsync("test-stream", 0, 1, false);
      var returnedEvent = streamEvents.Events[0].Event;

      Console.WriteLine("Read event with data: {0}, metadata: {1}",
        Encoding.UTF8.GetString(returnedEvent.Data),
        Encoding.UTF8.GetString(returnedEvent.Metadata)
      );
    }
  }
}
