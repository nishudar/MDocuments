using Common.DomainEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Common.Mediatr;

public class NotificationLoggingBehavior<TNotification, TResponse>(ILogger<NotificationLoggingBehavior<TNotification, TResponse>> logger)
    : IPipelineBehavior<TNotification, TResponse> where TNotification : notnull
{
    public async Task<TResponse> Handle(TNotification request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        switch (request)
        {
            case IDomainEvent:
            {
                logger.LogDebug("{Assembly} service handling domain event: {NotificationName} {@Notification}", request.GetType().Assembly.GetName().Name, typeof(TNotification).Name, request);
                var response = await next();
                logger.LogInformation("{Assembly} service finished domain event: {NotificationName} {@Response}", request.GetType().Assembly.GetName().Name, typeof(TNotification).Name, response);
                return response;
            }
            default:
            {
                logger.LogDebug("{Assembly} service handling command:  {NotificationName} {@Notification}", request.GetType().Assembly.GetName().Name,  typeof(TNotification).Name, request);
                var response = await next();
                logger.LogInformation("{Assembly} service finished handling command: {NotificationName} {@Response}", request.GetType().Assembly.GetName().Name, typeof(TNotification).Name, response);
                return response;
            }
        }
    }
}