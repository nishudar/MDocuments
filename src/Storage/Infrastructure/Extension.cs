using Microsoft.Extensions.DependencyInjection.Extensions;
using Storage.Application.Interfaces;

namespace Storage.Infrastructure;

internal static class Extension
{
    public static void AddInfrastructure(this IServiceCollection serviceCollection, string storageDirecotry)
    {
        serviceCollection.TryAddSingleton<IFileMetadataRepository, FileMetadataRepository>();
        serviceCollection.TryAddTransient<IFileReader>(_ => new FileReader(storageDirecotry));
        serviceCollection.TryAddTransient<IFileWriter>(_ => new FileWriter(storageDirecotry));
    }
}