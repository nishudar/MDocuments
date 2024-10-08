using MediatR;
using Storage.Application.Interfaces;
using Storage.Domain;

namespace Storage.Application.Queries;

internal record GetFileStatusQuery(Guid FileId) : IRequest<FileStatus?>;

internal class GetFileStatusHandler(IFileMetadataRepository fileMetadataRepository)
    : IRequestHandler<GetFileStatusQuery, FileStatus?>
{
    public Task<FileStatus?> Handle(GetFileStatusQuery request, CancellationToken cancellationToken)
    {
        return fileMetadataRepository.GetFileStatus(request.FileId, cancellationToken);
    }
}