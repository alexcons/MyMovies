using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public static class AppStopWatch
{
    private static List<KeyValuePair<string, TimeSpan>> times = new List<KeyValuePair<string, TimeSpan>>();
    private static Stopwatch watch = new Stopwatch();

    internal static void Start()
    {
        times.Clear();
        watch.Reset();
        watch.Start();
    }

    internal static void Stop()
    {
        watch.Stop();
    }

    internal static void Lap(string name)
    {
        times.Add(new KeyValuePair<string, TimeSpan>(name, watch.Elapsed));
    }

    internal static string GetLaps()
    {
        return string.Join("\n", (from t in times select t.Key + "=" + decimal.Round((decimal)t.Value.TotalMilliseconds, 0).ToString()).ToArray());
    }
}
