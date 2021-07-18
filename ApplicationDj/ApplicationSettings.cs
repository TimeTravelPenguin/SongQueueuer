#region Title Header

// Name: Phillip Smith
// 
// Solution: SongQueueue
// Project: ApplicationDj
// File Name: ApplicationSettings.cs
// 
// Current Data:
// 2021-07-18 8:30 PM
// 
// Creation Date:
// 2021-07-18 4:16 PM

#endregion

#region usings

using System;

#endregion

namespace ApplicationDj
{
  internal static class ApplicationSettings
  {
    public const string DbCount = "count";

    public static readonly string EntryAssemblyLocation = AppContext.BaseDirectory;
  }
}