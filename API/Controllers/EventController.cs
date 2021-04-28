using EventStore.ClientAPI;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using System.Threading.Tasks;

namespace API.Controllers
{
  [ApiController, Route("[controller]")]
  public class EventController : ControllerBase
  {
    public EventController() { }

    [HttpGet]
    public async Task Get()
    {
      var conn = EventStoreConnection.Create(new Uri("tcp://localhost:1113"));
      await conn.ConnectAsync();

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
      _ = await conn.AppendToStreamAsync(streamName, ExpectedVersion.Any, eventPayload);

      var readEvents = await conn.ReadStreamEventsForwardAsync(streamName, 0, 10, true);
      foreach (var evt in readEvents.Events)
      {
        Console.WriteLine(Encoding.UTF8.GetString(evt.Event.Data));
      }
    }
  }
}
