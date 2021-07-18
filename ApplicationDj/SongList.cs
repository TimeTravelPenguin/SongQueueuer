#region Title Header

// Name: Phillip Smith
// 
// Solution: TomSongQueueue
// Project: ApplicationDj
// File Name: SongList.cs
// 
// Current Data:
// 2021-07-18 4:44 PM
// 
// Creation Date:
// 2021-07-18 2:30 PM

#endregion

#region usings

using System.Collections.Generic;

#endregion

namespace ApplicationDj
{
  internal class SongList
  {
    public List<string> Songs { get; init; }

    public string this[int i]
    {
      get => Songs[i];
      set => Songs[i] = value;
    }
  }
}