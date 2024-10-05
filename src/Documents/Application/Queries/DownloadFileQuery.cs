using Documents.Application.Interfaces;
using Documents.Domain.Exceptions;
using Documents.Infrastructure.Clients.Storage.Models;

namespace Documents.Application.Queries;

using MediatR;

public record DownloadFileQuery(Guid DocumentId) : IRequest<FileDownloadResponse>;
    
public class DownloadFileHandler(IDocumentInventoryRepository repository, IStorageService storageService) : IRequestHandler<DownloadFileQuery,  FileDownloadResponse>
{
    public async Task<FileDownloadResponse> Handle(DownloadFileQuery request, CancellationToken cancellationToken)
    {
        var documentInventory = await repository.GetDocumentInventory(cancellationToken);
        var document = documentInventory.GetDocument(request.DocumentId);
        if (document is null && document is {FileId: null})
            throw new NotFoundException("document", request.DocumentId);
        var file = await storageService.DownloadFileAsync(document!.FileId!.Value, cancellationToken);
        if(file is null)
            throw new NotFoundException("document", request.DocumentId);
        
        return file;
    }
}