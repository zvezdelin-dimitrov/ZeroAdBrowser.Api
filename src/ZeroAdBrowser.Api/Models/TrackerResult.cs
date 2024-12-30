namespace ZeroAdBrowser.Api.Models;

using System.Collections.Generic;

public class TrackerResult
{
    public string Domain { get; set; }

    public string Default { get; set; }

    public List<RuleDefinitionResult> Rules { get; set; }
}
