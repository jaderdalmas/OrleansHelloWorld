﻿using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrainInterfaces
{
  public interface IHelloArchive : IGrainWithIntegerKey
  {
    Task<string> SayHello(string greeting);

    Task<IEnumerable<string>> GetGreetings();
  }
}
