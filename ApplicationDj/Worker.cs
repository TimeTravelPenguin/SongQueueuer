#region Title Header

// Name: Phillip Smith
// 
// Solution: TomSongQueueue
// Project: ApplicationDj
// File Name: Worker.cs
// 
// Current Data:
// 2021-07-18 4:44 PM
// 
// Creation Date:
// 2021-07-18 3:41 PM

#endregion

#region usings

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AllOverIt.GenericHost;
using AllOverIt.Process;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

#endregion

namespace ApplicationDj
{
  internal class Worker : ConsoleWorker
  {
    private static readonly Random Random = new();
    private readonly IDatabase _db;
    private readonly SongList _songs;
    private readonly ISubscriber _subscriber;

    public Worker(IHostApplicationLifetime applicationLifetime, ISubscriber subscriber, IDatabase db,
      SongList songs) : base(
      applicationLifetime)
    {
      _subscriber = subscriber;
      _db = db;
      _songs = songs;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      (await _subscriber.SubscribeAsync("song")).OnMessage(PlaySong);

      await Task.Run(() => { WaitHandle.WaitAny(new[] {stoppingToken.WaitHandle}); }, stoppingToken);
    }

    private async Task PlaySong(ChannelMessage obj)
    {
      _db.StringGet(Constants.DbCount).TryParse(out int currentCount);
      while (currentCount > 0)
      {
        var song = _songs[Random.Next(_songs.Songs.Count)];
        Console.WriteLine($"Playing the song '{Path.GetFileName(song)}'.");

        var args = $"--play-and-exit --qt-start-minimized \"{song}\"";

        _db.StringDecrement(Constants.DbCount);
        --currentCount;
        await Process.ExecuteAndWaitAsync(Environment.CurrentDirectory, "vlc.exe", args, -1);

        if (currentCount <= 0)
        {
          _db.StringGet(Constants.DbCount).TryParse(out currentCount);
        }
      }
    }
  }
}