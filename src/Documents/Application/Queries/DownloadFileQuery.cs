﻿using Documents.Application.Interfaces;
using Documents.Domain.Exceptions;
using Documents.Infrastructure;
using Documents.Infrastructure.Clients.Storage.Models;
using MediatR;

namespace Documents.Application.Queries;

internal record DownloadFileQuery(Guid DocumentId) : IRequest<FileDownloadResponse>;

internal class DownloadFileHandler(IDocumentsUnitOfWork unitOfWork, IStorageService storageService)
    : IRequestHandler<DownloadFileQuery, FileDownloadResponse>
{
    public async Task<FileDownloadResponse> Handle(DownloadFileQuery request, CancellationToken cancellationToken)
    {
        var documentInventory = await unitOfWork.GetDocumentInventory(cancellationToken);
        var document = documentInventory.GetDocument(request.DocumentId);
        if (document is null && document is {FileId: null})
            throw new NotFoundException("document", request.DocumentId);
        var file = await storageService.DownloadFileAsync(document!.FileId!.Value, cancellationToken);
        if (file is null)
            throw new NotFoundException("document", request.DocumentId);

        return file;
    }
}