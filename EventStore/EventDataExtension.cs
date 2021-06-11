using EventStore.Client;
using System.Text.Json;

namespace EventStore
{
  public static class EventDataExtension
  {
    public static EventData GetEvent(this uint number)
    {
      return new EventData(
        number.ToUuid(),
        number.GetType().ToString(),
        JsonSerializer.SerializeToUtf8Bytes(number)
      );
    }

    public static EventData GetEvent(this int number)
    {
      return new EventData(
        number.ToUuid(),
        number.GetType().ToString(),
        JsonSerializer.SerializeToUtf8Bytes(number)
      );
    }

    public static EventData GetEvent(this string text)
    {
      return new EventData(
        text.GetHashCode().ToUuid(),
        text.GetType().ToString(),
        JsonSerializer.SerializeToUtf8Bytes(text)
      );
    }

    public static EventData GetEvent(this object data, string type = "TestType")
    {
      return new EventData(Uuid.NewUuid(), type,
        JsonSerializer.SerializeToUtf8Bytes(data)
      );
    }
  }
}
