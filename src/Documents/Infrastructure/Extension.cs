using Documents.Application.Interfaces;
using Documents.Infrastructure.Clients.Storage;
using Refit;

namespace Documents.Infrastructure;

public static class Extension
{
    public static void AddInfrastructure(this IServiceCollection services, string storeServiceBaseUrl)
    {
        services.AddSingleton<IDocumentInventoryRepository, DocumentInventoryRepository>();
        //services.AddHostedService(KafkaConsumerBackgroundService);

        services.AddRefitClient<IStorageClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(storeServiceBaseUrl));

        services.AddTransient<IStorageService, StorageService>();
    }
}