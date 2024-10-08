﻿using Documents.Application.Queries;
using Documents.Domain.Entities;
using MediatR;

namespace Documents.Api;

internal static class UserEndpoints
{
    private const string TagUsers = "Users";

    public static void MapUserEndpoints(this IEndpointRouteBuilder app, TimeSpan timeout)
    {
        app.MapGet("/v1/document/user", async (
                IMediator mediator) =>
            {
                using var cts = new CancellationTokenSource(timeout);
                var users = await mediator.Send(new GetUsersQuery(), cts.Token);

                return Results.Ok(users);
            })
            .DisableAntiforgery()
            .WithName("getUsers")
            .WithTags(TagUsers)
            .Produces<IEnumerable<User>>()
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Get business users";
                operation.Description = "Get list of business users";
                return operation;
            });
    }
}