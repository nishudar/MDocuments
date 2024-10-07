using Documents.Application.Commands;
using Documents.Application.Queries;
using Documents.Domain.Entities;
using Documents.Domain.ValueObjects;
using MediatR;

namespace Documents.Api;

internal static class ProcessEndpoints
{
    private const string TagProcesses = "Processes";

    public static void MapProcessEndpoints(this IEndpointRouteBuilder app, TimeSpan timeout)
    {
        app.MapGet("/v1/document/process", async (
                IMediator mediator) =>
            {
                using var cts = new CancellationTokenSource(timeout);
                var processes = await mediator.Send(new GetProcessesQuery(), cts.Token);

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

        app.MapPost("/v1/documents/{operatorId:guid}/{customerId:guid}/abandon", async (
                Guid operatorId,
                Guid customerId,
                IMediator mediator) =>
            {
                using var cts = new CancellationTokenSource(timeout);
                await mediator.Send(new AbandonProcessCommand(operatorId, customerId), cts.Token);

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


        app.MapPost("/v1/documents/{operatorId:guid}/{customerId:guid}/start", async (
                Guid operatorId,
                Guid customerId,
                IMediator mediator) =>
            {
                using var cts = new CancellationTokenSource(timeout);
                var report = await mediator.Send(new StartProcessCommand(operatorId, customerId), cts.Token);

                return Results.Ok(report);
            })
            .DisableAntiforgery()
            .WithName("startProcess")
            .WithTags(TagProcesses)
            .Produces<ProcessReport>()
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Start a document process";
                operation.Description = "Start process of sending documents";

                return operation;
            });

        app.MapPost("/v1/documents/{operatorId:guid}/{customerId:guid}/finish", async (
                Guid operatorId,
                Guid customerId,
                IMediator mediator) =>
            {
                using var cts = new CancellationTokenSource(timeout);
                await mediator.Send(new FinishProcessCommand(operatorId, customerId), cts.Token);

                return Results.NoContent();
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
                IMediator mediator) =>
            {
                using var cts = new CancellationTokenSource(timeout);
                var report = await mediator.Send(new TrackProcessQuery(processId), cts.Token);

                return Results.Ok(report);
            })
            .WithName("trackProcess")
            .WithTags(TagProcesses)
            .Produces<ProcessReport>()
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Track a document process";
                operation.Description = "Retrieves the status report of a document process";

                return operation;
            });
    }
}