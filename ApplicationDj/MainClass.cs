#region Title Header

// Name: Phillip Smith
// 
// Solution: TomSongQueueue
// Project: ApplicationDj
// File Name: MainClass.cs
// 
// Current Data:
// 2021-07-18 4:44 PM
// 
// Creation Date:
// 2021-07-18 3:41 PM

#endregion

#region usings

using System;
using System.Threading;
using System.Threading.Tasks;
using AllOverIt.GenericHost;

#endregion

namespace ApplicationDj
{
  internal class MainClass : ConsoleAppBase
  {
    public override Task StartAsync(CancellationToken cancellationToken = new())
    {
      Console.WriteLine("Press ESC to exit");

      while (true)
      {
        var kp = Console.ReadKey(true);

        if (kp.Key == ConsoleKey.Escape)
        {
          break;
        }
      }

      return Task.CompletedTask;
    }
  }
}