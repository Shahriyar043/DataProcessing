using DataProcessing.Api.Services.DataProcessor;
using DataProcessing.Api.Services.DataReceiver;
using DataProcessing.Api.Services.DataSender;

namespace DataProcessing.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IDataProcessorService, DataProcessorService>();
        services.AddScoped<IDataSenderService, DataSenderService>();
        services.AddScoped<IDataReceiverService, DataReceiverService>();
        
        return services;
    }
}
