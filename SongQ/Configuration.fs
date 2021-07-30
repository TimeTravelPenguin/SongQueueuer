namespace SongQ

open System.IO
open Newtonsoft.Json

[<AutoOpen>]
module internal Configuration = 
  let ParseAppConfig input =
    JsonConvert.DeserializeObject<ApplicationConfiguration>(input)

  let LoadConfiguration configPath = 
    File.ReadAllText >> ParseAppConfig