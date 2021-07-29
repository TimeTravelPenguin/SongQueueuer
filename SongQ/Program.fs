open System.Collections.Generic
open Newtonsoft.Json
open System.IO
open AllOverIt.GenericHost
open System
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Hosting
open System.Threading.Tasks
open System.Threading

[<AutoOpen>]
module internal ApplicationTypes =
  type TwitchConfiguration = {
    ApplicationClientId : string
    ApplicationClientSecret : string
    ChannelName : string
    OAuthAccessToken : string
  }

  type Song = {
    Path : string
    PlayOnBits : int option
    PlayOnRedemption : string option
  }

  type Playlist = {
    Songs : IEnumerable<Song>
    PlayOnBits : int option
    PlayOnRedemption : string option
  }
  
  type ApplicationConfiguration = {
    Configuration : TwitchConfiguration
    Songs : IEnumerable<Song>
    Playlists : IEnumerable<Song>
}

[<AutoOpen>]
module internal ConsoleHosting =
  type ConsoleHost(logger) =
    inherit ConsoleAppBase()
    member private _.logger : ILogger<ConsoleHost> = logger
    
    override this.StartAsync cancellationToken = 
      printfn "SongQ"
      this.logger.LogInformation "Logging"
      
      let mutable contLoop = true
      while contLoop do
        let key = Console.ReadKey(true).Key
        match key with
        | ConsoleKey.Escape -> contLoop <- false
        | _ -> ()

      Task.CompletedTask

[<AutoOpen>]
module internal Configuration = 
  let ParseAppConfig input =
    JsonConvert.DeserializeObject<ApplicationConfiguration>(input)

  let LoadConfiguration configPath = 
    File.ReadAllText >> ParseAppConfig

let ConstructHost() = 
  GenericHost.CreateConsoleHostBuilder()
    .ConfigureServices(fun _ service -> 
      service.AddSingleton<IConsoleApp, ConsoleHost>()
      |> ignore
    )

[<EntryPoint>]
let main argv =
    let host = ConstructHost()
    async {
      return! host.RunConsoleAsync((fun options -> options.SuppressStatusMessages <- true), CancellationToken.None)
      |> Async.AwaitTask
    } |> Async.RunSynchronously
    0