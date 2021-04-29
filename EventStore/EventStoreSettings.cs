namespace EventStore
{
  public class EventStoreSettings
  {
    /// <summary>
    /// Default: esdb://localhost:2113?tls=false
    /// </summary>
    public string Connection { get; set; }
  }
}
