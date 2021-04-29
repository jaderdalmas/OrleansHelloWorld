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
    private readonly EventStoreClient _client;
    private string EventType => "TestType";
    private string TestStream => "test-stream";

    public EventController(EventStoreClient client)
    {
      _client = client;
    }

    [HttpGet("{stream}")]
    public async Task<string> GetAll(string stream)
    {
      var result = _client.ReadStreamAsync(
        Direction.Forwards,
        string.IsNullOrWhiteSpace(stream) ? TestStream : stream,
        StreamPosition.Start
      );

      if (await result.ReadState == ReadState.StreamNotFound)
        return null;

      var sb = new StringBuilder();
      foreach (var vnt in result.ToEnumerable())
        sb.AppendLine(Encoding.UTF8.GetString(vnt.Event.Data.Span));

      return sb.ToString();
    }

    [HttpGet]
    public async Task Post(string message, string stream)
    {
      if (string.IsNullOrWhiteSpace(message))
        message = "I wrote my first event!";

      var data = new
      {
        EntityId = Guid.NewGuid().ToString("N"),
        ImportantData = message
      };

      var eventData = new EventData(
        Uuid.NewUuid(),
        EventType,
        JsonSerializer.SerializeToUtf8Bytes(data)
      );

      await _client.AppendToStreamAsync(
        string.IsNullOrWhiteSpace(stream) ? TestStream : stream,
        StreamState.Any,
        new[] { eventData }
      );
    }
  }
}
