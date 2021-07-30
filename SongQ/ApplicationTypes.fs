namespace SongQ

open System.Collections.Generic

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
    Playlists : IEnumerable<Playlist>
}