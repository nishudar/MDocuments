using MediatR;
using Microsoft.Extensions.Logging;

namespace Common.Mediatr;

public class RequestLoggingBehavior<TRequest, TResponse>(ILogger<RequestLoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        string requestTypeString;
        if (request.GetType().Name.EndsWith("Query"))
            requestTypeString = "query";
        else if (request.GetType().Name.EndsWith("Command"))
            requestTypeString = "command";
        else
            requestTypeString = "request"

        logger.LogDebug("{Assembly} service Handling {RequestTypeString}: {RequestName} {@Request}",
            request.GetType().Assembly.GetName().Name, requestTypeString, typeof(TRequest).Name, request);
        var response = await next();
        logger.LogInformation("{Assembly} service Finished handling {RequestTypeString}: {RequestName} {@Response}",
            request.GetType().Assembly.GetName().Name, requestTypeString, typeof(TRequest).Name, response);

        return response;
    }
}