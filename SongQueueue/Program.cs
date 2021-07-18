#region Title Header

// Name: Phillip Smith
// 
// Solution: TomSongQueueue
// Project: SongQueuer
// File Name: Program.cs
// 
// Current Data:
// 2021-07-18 4:44 PM
// 
// Creation Date:
// 2021-07-18 11:06 AM

#endregion

#region usings

using System;
using StackExchange.Redis;

#endregion

namespace SongQueuer
{
  public static class Program
  {
    private const string DbCount = "count";
    private static ConnectionMultiplexer _redis;
    private static IDatabase _db;

    public static void Main()
    {
      IncrementRedisCount();

      var sub = _redis.GetSubscriber();
      sub.Publish("song", $"Song published: {DateTime.Now}");
    }

    private static void IncrementRedisCount()
    {
      Console.WriteLine("Connecting to redis...");
      try
      {
        _redis = ConnectionMultiplexer.Connect("localhost:5050");
      }
      catch (Exception e)
      {
        Console.WriteLine("Unable to connect to redis" + Environment.NewLine);
        Console.WriteLine(e);
        Environment.Exit(0);
      }

      Console.WriteLine("Successfully connected to redis");
      _db = _redis.GetDatabase();

      var count = _db.StringGet(DbCount);

      if (count == RedisValue.Null)
      {
        _db.StringSet(new RedisKey(DbCount), 0);
      }
      else
      {
        _db.StringIncrement(DbCount);
      }
    }
  }
}