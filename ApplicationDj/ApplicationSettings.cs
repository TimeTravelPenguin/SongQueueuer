#region Title Header

// Name: Phillip Smith
// 
// Solution: SongQueueue
// Project: ApplicationDj
// File Name: ApplicationSettings.cs
// 
// Current Data:
// 2021-07-18 7:18 PM
// 
// Creation Date:
// 2021-07-18 4:16 PM

#endregion

#region usings

using System;
using System.Reflection;

#endregion

namespace ApplicationDj
{
  internal static class ApplicationSettings
  {
    public const string DbCount = "count";

    public static readonly string EntryAssemblyLocation = Assembly.GetEntryAssembly()?.Location ??
                                                          throw new NullReferenceException("Entry assembly is null");
  }
}