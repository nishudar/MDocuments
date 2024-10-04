using Common.Abstracts;
using MediatR;
using Storage.Application.Interfaces;

namespace Storage.Application.Queries;
public record GetFileStatusQuery(Guid FileId) : IRequest<FileStatus?>;

public class GetFileStatusHandler(IFileMetadataRepository fileMetadataRepository) : IRequestHandler<GetFileStatusQuery, FileStatus?>
{
    public Task<FileStatus?> Handle(GetFileStatusQuery request, CancellationToken cancellationToken) 
        => fileMetadataRepository.GetFileStatus(request.FileId, cancellationToken);
}