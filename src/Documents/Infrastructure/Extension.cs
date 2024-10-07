using Documents.Application.Interfaces;
using Documents.Infrastructure.Clients.Storage;
using MediatR;
using Refit;

namespace Documents.Infrastructure;

internal static class Extension
{
    public static void AddInfrastructure(
        this IServiceCollection services, 
        string storeServiceBaseUrl,
        string kafkaServer)
    {
        services.AddSingleton<IDocumentInventoryRepository, DocumentInventoryRepository>();
        services.AddTransient<IStorageService, StorageService>();
        
        services.AddHostedService(sp => new KafkaConsumerBackgroundService(
            sp.GetRequiredService<ILogger<KafkaConsumerBackgroundService>>(),
            sp.GetRequiredService<IMediator>(),
            kafkaServer));

        services.AddRefitClient<IStorageClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(storeServiceBaseUrl));
    }
}