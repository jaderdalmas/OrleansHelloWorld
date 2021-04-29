using Interfaces;
using Orleans;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client
{
  public static class HelloWorld
  {
    public static async Task DoHello(IClusterClient client, int counter)
    {
      var friend = client.GetGrain<IHello>(0);
      var response = await friend.SayHello($"Good morning, {counter}!");
      Console.WriteLine($"\n\n{response}\n\n");
    }

    public static void ManyHello(IClusterClient client)
    {
      var tasks = new List<Task>();
      for (int i = 1; i < 100; i++)
        tasks.Add(DoHello(client, i));

      Task.WaitAll(tasks.ToArray());
    }
  }
}
