namespace SongQ

open Microsoft.Extensions.Logging
open AllOverIt.GenericHost
open System
open System.Threading.Tasks
open Microsoft.Extensions.Hosting;
open TwitchLib.Api.Interfaces
open TwitchLib.PubSub.Interfaces
open System.Collections.Generic
open System.Linq
open TwitchLib.Api.Helix.Models.Users.GetUsers
open System.Threading

[<AutoOpen>]
module internal ConsoleModels =
  type ConsoleHost(logger) =
    inherit ConsoleAppBase()
    member private _.logger : ILogger<ConsoleHost> = logger

    override this.StartAsync _ =
      this.logger.LogInformation $"Executing {(nameof this.StartAsync)}"

      let mutable contLoop = true
      while contLoop do
        let key = Console.ReadKey(true).Key
        match key with
        | ConsoleKey.Escape -> contLoop <- false
        | _ -> ()

      Task.CompletedTask

  type TwitchApiDj(applicationLifetime : IHostApplicationLifetime,
    appConfig : ApplicationConfiguration,
    twitchApi : ITwitchAPI,
    twitchClient : ITwitchPubSub,
    logger : ILogger<TwitchApiDj>) =
      inherit ConsoleWorker(applicationLifetime)
      member private _.applicationLifetime = applicationLifetime
      member private _.appConfig = appConfig
      member private _.twitchApi = twitchApi
      member private _.twitchClient = twitchClient
      member private _.logger = logger

      member private _.ConfigureListeningEvents() =
        let userData =
          twitchApi.Helix.Users.GetUsersAsync(logins = List<string> [appConfig.Configuration.ChannelName])
          |> Async.AwaitTask
          |> Async.RunSynchronously
          |> fun (user : GetUsersResponse) -> Array.tryHead user.Users
        Task.CompletedTask

      override this.ExecuteAsync (ct : CancellationToken) =
        this.ConfigureListeningEvents()