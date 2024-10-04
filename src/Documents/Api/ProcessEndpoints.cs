using Common.DomainEvents;
using Documents.Application.Interfaces;
using Documents.Domain.Entities;
using Documents.Domain.ValueTypes;

namespace Documents.Api;

public static class ProcessEndpoints
{
    private const string TagProcesses = "Processes";
    
    public static void MapProcessEndpoints(this IEndpointRouteBuilder app, TimeSpan timeout)
    {
        app.MapGet("/v1/document/process", async (
                IDocumentInventoryRepository repository) =>
            {
                using var cts = new CancellationTokenSource(timeout);
                var documentInventory = await repository.GetDocumentInventory(cts.Token);
                var processes = documentInventory.GetProcesses();

                return Results.Ok(processes);
            })
            .DisableAntiforgery()
            .WithName("getProcesses")
            .WithTags(TagProcesses)
            .Produces<IEnumerable<Process>>()
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Get list of processes";
                operation.Description = "Get list of processes and associated data";
                return operation;
            });

        
        app.MapPost("/v1/documents/{userId:guid}/{customerId:guid}/abandon", async (
                Guid userId,
                Guid customerId,
                IDocumentInventoryRepository repository,
                IDomainEventDispatcher dispatcher) =>
            {
                using var cts = new CancellationTokenSource(timeout);
                var documentInventory = await repository.GetDocumentInventory(cts.Token);
                documentInventory.AbandonProcess(userId, customerId);
                await dispatcher.DispatchEvents(documentInventory.BusinessEvents, cts.Token);

                return Results.NoContent();
            })
            .DisableAntiforgery()
            .WithName("abandonProcess")
            .WithTags(TagProcesses)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Abandon a document process";
                operation.Description = "Marks the document process as abandoned for a user and customer.";

                return operation;
            });
        
        
        app.MapPost("/v1/documents/{userId:guid}/{customerId:guid}/start", async (
                Guid userId,
                Guid customerId,
                IDocumentInventoryRepository repository,
                IDomainEventDispatcher dispatcher) =>
            {
                using var cts = new CancellationTokenSource(timeout);
                var documentInventory = await repository.GetDocumentInventory(cts.Token);
                var process = documentInventory.StartProcess(userId, customerId);
                await dispatcher.DispatchEvents(documentInventory.BusinessEvents, cts.Token);

                return Results.Ok(documentInventory.GetReport(process.Id));
            })
            .DisableAntiforgery()
            .WithName("startProcess")
            .WithTags(TagProcesses)
            .Produces<ProcessReport>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Start a document process";
                operation.Description = "Start process of sending documents";

                return operation;
            });

        app.MapPost("/v1/documents/{userId:guid}/{customerId:guid}/finish", async (
                Guid userId,
                Guid customerId,
                IDocumentInventoryRepository repository,
                IDomainEventDispatcher dispatcher) =>
            {
                using var cts = new CancellationTokenSource(timeout);
                var documentInventory = await repository.GetDocumentInventory(cts.Token);
                documentInventory.FinishProcess(userId, customerId);
                await dispatcher.DispatchEvents(documentInventory.BusinessEvents, cts.Token);
            })
            .DisableAntiforgery()
            .WithName("finishProcess")
            .WithTags(TagProcesses)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Finish a document process";
                operation.Description = "Marks the document process as finished for a user and customer.";

                return operation;
            });

        app.MapGet("/v1/documents/{processId:guid}/track", async (
                Guid processId,
                IDocumentInventoryRepository repository,
                IDomainEventDispatcher dispatcher) =>
            {
                using var cts = new CancellationTokenSource(timeout);
                var documentInventory = await repository.GetDocumentInventory(cts.Token);
                var report = documentInventory.GetReport(processId);
                await dispatcher.DispatchEvents(documentInventory.BusinessEvents, cts.Token);
                
                return Results.Ok(report);
            })
            .WithName("trackProcess")
            .WithTags(TagProcesses)
            .Produces<ProcessReport>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Track a document process";
                operation.Description = "Retrieves the status report of a document process";

                return operation;
            });
    }
}
