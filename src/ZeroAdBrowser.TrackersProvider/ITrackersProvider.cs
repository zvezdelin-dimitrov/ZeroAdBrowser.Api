using System.Collections.Generic;
using System.Threading.Tasks;
using ZeroAdBrowser.TrackersProvider.Models;

public interface ITrackersProvider
{
    Task<List<TrackerResult>> GetTrackers();
}
