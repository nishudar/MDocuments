using MediatR;
using Storage.Application.Interfaces;
using Storage.Domain;

namespace Storage.Application.Queries;

internal record GetFilesMetadataQuery(Guid? UserId) : IRequest<IEnumerable<FileMetadata>>;

internal class GetAllFilesHandler(IFileMetadataRepository fileMetadataRepository)
    : IRequestHandler<GetFilesMetadataQuery, IEnumerable<FileMetadata>>
{
    public async Task<IEnumerable<FileMetadata>> Handle(GetFilesMetadataQuery request,
        CancellationToken cancellationToken)
    {
        return await fileMetadataRepository.GetAllUserFileMetadata(request.UserId, cancellationToken);
    }
}