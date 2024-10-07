using MediatR;
using Storage.Application.Interfaces;
using Storage.Domain;

namespace Storage.Application.Queries;

internal record GetFileMetadataQuery(Guid FileId) : IRequest<FileMetadata?>;

internal class GetFileMetadataHandler(IFileMetadataRepository fileMetadata)
    : IRequestHandler<GetFileMetadataQuery, FileMetadata?>
{
    public Task<FileMetadata?> Handle(GetFileMetadataQuery request, CancellationToken cancellationToken)
    {
        return fileMetadata.GetFileMetadata(request.FileId, cancellationToken);
    }
}