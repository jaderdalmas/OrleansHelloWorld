using EventStore;
using EventStore.Client;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Controllers
{
  [ApiController, Route("[controller]")]
  public class EventController : ControllerBase
  {
    private EventStoreClient Client { get; }
    private string EventType => "TestType";
    private string TestStream => "test-stream";

    public EventController(IEventStoreService eventStoreService)
    {
      Client = eventStoreService.Client;
    }

    [HttpGet]
    public async Task Get()
    {
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

      await Client.AppendToStreamAsync(
        TestStream,
        StreamState.Any,
        new[] { eventData }
      );

      var result = Client.ReadStreamAsync(
        Direction.Forwards,
        TestStream,
        StreamPosition.Start
      );

      if (await result.ReadState == ReadState.StreamNotFound)
        return;

      foreach (var vnt in result.ToEnumerable())
        Console.WriteLine(Encoding.UTF8.GetString(vnt.Event.Data.Span));
    }
  }
}
