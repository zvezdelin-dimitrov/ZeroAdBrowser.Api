namespace ZeroAdBrowser.TrackersProvider.Models;

using System.Collections.Generic;

internal class TrackerList
{
    public Dictionary<string, Tracker> Trackers { get; set; }

    public Dictionary<string, Entity> Entities { get; set; }

    public Dictionary<string, string> Domains { get; set; }

    public Dictionary<string, string> CNames { get; set; }
}
