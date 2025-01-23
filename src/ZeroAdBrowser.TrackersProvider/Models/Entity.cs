namespace ZeroAdBrowser.TrackersProvider.Models;

using System.Collections.Generic;

internal class Entity
{
    public List<string> Domains { get; set; }

    public double Prevalence { get; set; }

    public string DisplayName { get; set; }
}
