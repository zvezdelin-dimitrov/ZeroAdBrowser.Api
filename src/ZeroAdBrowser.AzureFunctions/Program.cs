using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args).ConfigureFunctionsWebApplication();

builder.Services.AddTrackersProvider(builder.Configuration);

builder.Build().Run();
