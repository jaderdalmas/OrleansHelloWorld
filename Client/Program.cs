﻿using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client
{
  public class Program
  {
    public static int Main(string[] args)
    {
      return RunMainAsync().Result;
    }

    private static async Task<int> RunMainAsync()
    {
      try
      {
        var tasks = new List<Task>();
        using (var client = await ConnectClient())
        {
          for (int i = 0; i < 100; i++)
            tasks.Add(DoClientWork(client));

          Task.WaitAll(tasks.ToArray());
          Console.ReadKey();
        }

        return 0;
      }
      catch (Exception e)
      {
        Console.WriteLine($"\nException while trying to run client: {e.Message}");
        Console.WriteLine("Make sure the silo the client is trying to connect to is running.");
        Console.WriteLine("\nPress any key to exit.");
        Console.ReadKey();
        return 1;
      }
    }

    private static async Task<IClusterClient> ConnectClient()
    {
      IClusterClient client;
      client = new ClientBuilder()
          .UseLocalhostClustering()
          .Configure<ClusterOptions>(options =>
          {
            options.ClusterId = "dev";
            options.ServiceId = "OrleansBasics";
          })
          .ConfigureLogging(logging => logging.AddConsole())
          .Build();

      await client.Connect();
      Console.WriteLine("Client successfully connected to silo host \n");
      return client;
    }

    private static async Task DoClientWork(IClusterClient client)
    {
      // example of calling grains from the initialized client
      var friend = client.GetGrain<IHello>(0);
      var response = await friend.SayHello("Good morning, HelloGrain!");
      Console.WriteLine($"\n\n{response}\n\n");
    }
  }
}
