using MediatR;
using Storage.Application.Interfaces;
using Storage.Domain;

namespace Storage.Application.Queries;

public record GetFilesMetadataQuery(Guid? UserId) : IRequest<IEnumerable<FileMetadata>>;

public class GetAllFilesHandler(IFileMetadataRepository fileMetadataRepository)
    : IRequestHandler<GetFilesMetadataQuery, IEnumerable<FileMetadata>>
{
    public async Task<IEnumerable<FileMetadata>> Handle(GetFilesMetadataQuery request, CancellationToken cancellationToken)
        => await fileMetadataRepository.GetAllUserFileMetadata(request.UserId, cancellationToken);
}