using MediatR;
using Storage.Application.Interfaces;
using Storage.Domain;

namespace Storage.Application.Queries;

public record GetFileMetadataQuery(Guid FileId) : IRequest<FileMetadata?>;

public class GetFileMetadataHandler(IFileMetadataRepository fileMetadata)
    : IRequestHandler<GetFileMetadataQuery, FileMetadata?>
{
    public Task<FileMetadata?> Handle(GetFileMetadataQuery request, CancellationToken cancellationToken) 
        => fileMetadata.GetFileMetadata(request.FileId, cancellationToken);
}