using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Threading.Tasks;

internal class AzureFunctions(ITrackersProvider trackersProvider, IOptions<JsonSerializerOptions> jsonSerializerOptions)
{
    [Function(nameof(Trackers))]
    public async Task<IActionResult> Trackers([HttpTrigger(AuthorizationLevel.Anonymous, nameof(HttpMethods.Get))] HttpRequest request)
    {
        var trackers = await trackersProvider.GetTrackers();
        return new JsonResult(trackers, jsonSerializerOptions);
    }
}
