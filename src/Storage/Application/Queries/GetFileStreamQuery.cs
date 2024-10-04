using Common.Abstracts;
using MediatR;
using Storage.Application.Interfaces;

namespace Storage.Application.Queries;

public record GetFileStreamQuery(Guid FileId) : IRequest<IFileReader.FileStreamResult?>;

public class GetFileByIdHandler(IFileReader fileReader, IFileMetadataRepository fileMetadata) : IRequestHandler<GetFileStreamQuery, IFileReader.FileStreamResult?>
{
    public async Task<IFileReader.FileStreamResult?> Handle(GetFileStreamQuery request, CancellationToken ct)
    {
        var metadata = await fileMetadata.GetFileMetadata(request.FileId, ct);
        var result = metadata?.Status is FileStatus.Completed ? fileReader.GetFile(metadata) : null;
        
        return result;
    }
}
