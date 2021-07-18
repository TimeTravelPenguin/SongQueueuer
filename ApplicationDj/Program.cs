#region Title Header

// Name: Phillip Smith
// 
// Solution: SongQueueue
// Project: ApplicationDj
// File Name: Program.cs
// 
// Current Data:
// 2021-07-18 7:21 PM
// 
// Creation Date:
// 2021-07-18 11:40 AM

#endregion

#region usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AllOverIt.GenericHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using StackExchange.Redis;

#endregion

namespace ApplicationDj
{
  public static class Program
  {
    private static ConnectionMultiplexer _redis;
    private static IDatabase _db;
    private static SongList _songs;

    public static async Task Main(string[] args)
    {
      CheckSongPaths();
      InitRedis();
      var sub = _redis.GetSubscriber();

      await CreateHostBuilder(args)
        .ConfigureServices(services =>
        {
          services.AddHostedService<Worker>();
          services.AddScoped(p => sub);
          services.AddScoped(p => _db);
          services.AddScoped(p => _songs);
        })
        .RunConsoleAsync(options => options.SuppressStatusMessages = true);
    }

    private static void CheckSongPaths()
    {
      var file = Path.Combine(ApplicationSettings.EntryAssemblyLocation, "songs.json");
      var fileExists = File.Exists(file);

      if (!fileExists)
      {
        Console.WriteLine("Please add songs to 'songs.json'.");

        var data = JsonConvert.SerializeObject(
          new SongList {Songs = new List<string> {@"path\\to\\song1.wav", @"path\\to\\song2.mp3", "..."}},
          Formatting.Indented);
        File.WriteAllText(file, data);

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey(true);
        Environment.Exit(0);
      }

      var fileData = File.ReadAllText(file);
      SongList songs;
      try
      {
        songs = JsonConvert.DeserializeObject<SongList>(fileData,
                  new JsonSerializerSettings {StringEscapeHandling = StringEscapeHandling.Default}) ??
                throw new InvalidOperationException("Unable to deserialize songs list.");
      }
      catch (Exception e)
      {
        Console.WriteLine("There is an error with the formatting of 'songs.json'." + Environment.NewLine);
        Console.WriteLine(e.Message);
        Environment.Exit(0);
        return;
      }

      foreach (var song in songs.Songs)
      {
        var valid = File.Exists(song) && (Path.HasExtension(".wav") || Path.HasExtension(".mp3"));

        if (!valid)
        {
          Console.WriteLine($"The song '{song}' is invalid. Ensure file exists and is .wav or .mp3 format.");
          Console.WriteLine("Press any key to exit...");
          Console.ReadKey(true);
          Environment.Exit(0);
        }
      }

      _songs = songs;
    }

    private static void InitRedis()
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

      var count = _db.StringGet(ApplicationSettings.DbCount);

      if (count == RedisValue.Null)
      {
        _db.StringAppend(new RedisKey(ApplicationSettings.DbCount), 0);
      }
      else
      {
        InitialiseDbReset();
      }
    }

    private static void InitialiseDbReset()
    {
      while (true)
      {
        Console.WriteLine("The database currently has songs queued.");
        Console.Write("Would you like to clear it? [Y/N]: ");

        var kp = Console.ReadLine() ?? throw new InvalidOperationException();

        switch (kp.ToLowerInvariant())
        {
          case "n":
            return;
          case "y":
          {
            _db.StringSet(ApplicationSettings.DbCount, "0");
            Console.WriteLine("Cleared song queue.");
            return;
          }
        }
      }
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
      return GenericHost
        .CreateConsoleHostBuilder(args)
        .ConfigureServices((hostContext, services) => { services.AddScoped<IConsoleApp, MainClass>(); });
    }
  }
}