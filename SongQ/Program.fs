namespace SongQ

open AllOverIt.GenericHost
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open TwitchLib.Api.Interfaces
open TwitchLib.PubSub.Interfaces
open TwitchLib.PubSub
open TwitchLib.Api
open System.Collections.Generic
open System

module internal Main =
#if DEBUG
  let demoConfig : ApplicationConfiguration = {
    Configuration = {
      ApplicationClientId = ""
      ApplicationClientSecret = ""
      ChannelName = "TimeTravelPenguin"
      OAuthAccessToken = ""
    }
    Songs = List<Song> []
    Playlists = List<Playlist> []
  }
#endif

  let ConstructHost() =
    GenericHost.CreateConsoleHostBuilder()
      .ConfigureServices(fun service ->
        service.AddSingleton<IConsoleApp, ConsoleHost>() |> ignore
        service.AddHostedService<TwitchApiDj>() |> ignore
        service.AddSingleton<ApplicationConfiguration>(fun (p : IServiceProvider) -> demoConfig) |> ignore
        service.AddScoped<ITwitchAPI, TwitchAPI>() |> ignore
        service.AddScoped<ITwitchPubSub, TwitchPubSub>() |> ignore
      )

  [<EntryPoint>]
  let main argv =
      let host = ConstructHost()
      async {
        return host.RunConsoleAsync(fun options -> options.SuppressStatusMessages <- true)
      } 
      |> Async.RunSynchronously |> ignore
      0