using Documents.Application.IntegrationEventsHandler;
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
        
        services.AddHostedService(sp => new IntegrationEventsHandlerService(
            sp.GetRequiredService<ILogger<IntegrationEventsHandlerService>>(),
            sp.GetRequiredService<IMediator>(),
            kafkaServer));

        services.AddRefitClient<IStorageClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(storeServiceBaseUrl));
    }
}