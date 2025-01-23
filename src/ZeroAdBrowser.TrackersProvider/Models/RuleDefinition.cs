namespace ZeroAdBrowser.TrackersProvider.Models;

internal class RuleDefinition
{
    public string Rule { get; set; }

    public string Surrogate { get; set; }

    public string Action { get; set; }

    public Exceptions Exceptions { get; set; }
}
