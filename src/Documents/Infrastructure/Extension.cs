using Documents.Application.IntegrationEventsHandler;
using Documents.Application.Interfaces;
using Documents.Domain.Aggregates;
using Documents.Infrastructure.Clients.Storage;
using Documents.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Refit;

namespace Documents.Infrastructure;

internal static class Extension
{
    public static void AddInfrastructure(
        this IServiceCollection services, 
        string storeServiceBaseUrl,
        string kafkaServer)
    {
        services.AddTransient<IStorageService, StorageService>();
        
        services.AddHostedService(sp => new IntegrationEventsHandlerService(
            sp.GetRequiredService<ILogger<IntegrationEventsHandlerService>>(),
            sp.GetRequiredService<IMediator>(),
            kafkaServer));

        services.AddRefitClient<IStorageClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(storeServiceBaseUrl));
        
        services.AddDbContext<DocumentsServiceContext>(options =>
        {
            options.UseInMemoryDatabase("DocumentsDatabase")
                .EnableSensitiveDataLogging();
        });
        services.AddTransient<IDocumentsUnitOfWork, DocumentsUnitOfWork>();
    }
    
    public static void UseInfrastructure(this WebApplication webApplication)
    {
        using var scope = webApplication.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DocumentsServiceContext>();
        dbContext.Database.EnsureCreated();
    }
    
    public static async Task<IDocumentsInventory> GetDocumentInventory(this IDocumentsUnitOfWork unitOfWork, CancellationToken ct = default)
    {
        var users = await unitOfWork.GetUsers(ct);
        var allowedDocumentTypes = await unitOfWork.GetDocumentTypes(ct);
        var processes = await unitOfWork.GetProcesses(ct);

        return new DocumentsInventory(users, allowedDocumentTypes, processes);
    }
}