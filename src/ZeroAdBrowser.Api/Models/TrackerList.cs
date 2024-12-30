namespace ZeroAdBrowser.Api.Models;

using System.Collections.Generic;

public class TrackerList
{
    public Dictionary<string, Tracker> Trackers { get; set; }

    public Dictionary<string, Entity> Entities { get; set; }

    public Dictionary<string, string> Domains { get; set; }

    public Dictionary<string, string> CNames { get; set; }
}
