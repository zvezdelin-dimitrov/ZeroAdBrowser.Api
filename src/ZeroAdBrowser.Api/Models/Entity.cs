namespace ZeroAdBrowser.Api.Models;

using System.Collections.Generic;

public class Entity
{    
    public List<string> Domains { get; set; }

    public double Prevalence { get; set; }

    public string DisplayName { get; set; }
}
