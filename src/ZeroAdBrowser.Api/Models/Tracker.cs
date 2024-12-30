namespace ZeroAdBrowser.Api.Models;

using System.Collections.Generic;

public class Tracker
{
    public string Domain { get; set; }

    public Owner Owner { get; set; }

    public double Prevalence { get; set; }

    public int Fingerprinting { get; set; }

    public double Cookies { get; set; }

    public List<string> Categories { get; set; }

    public string Default { get; set; }

    public List<RuleDefinition> Rules { get; set; }
}
