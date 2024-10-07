using Documents.Api.Models;
using Documents.Application.Commands;
using Documents.Application.Queries;
using Documents.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Documents.Api;

internal static class UserEndpoints
{
    private const string TagBusinessUsers = "Business Users";

    public static void MapUserEndpoints(this IEndpointRouteBuilder app, TimeSpan timeout)
    {
        app.MapGet("/v1/document/user", async (
                IMediator mediator) =>
            {
                using var cts = new CancellationTokenSource(timeout);
                var users = await mediator.Send(new GetBusinessUsersQuery(), cts.Token);

                return Results.Ok(users);
            })
            .DisableAntiforgery()
            .WithName("getUsers")
            .WithTags(TagBusinessUsers)
            .Produces<IEnumerable<BusinessUser>>()
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Get business users";
                operation.Description = "Get list of business users";
                return operation;
            });

        app.MapPost("/v1/document/user", async (
                [FromBody] AddUserModel businessUser,
                IMediator mediator) =>
            {
                using var cts = new CancellationTokenSource(timeout);
                var user = await mediator.Send(new AddBusinessUserCommand(businessUser.Name), cts.Token);

                return Results.Ok(new IdResponse(user.Id));
            })
            .DisableAntiforgery()
            .WithName("addUser")
            .WithTags(TagBusinessUsers)
            .Produces<IdResponse>()
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Add a business user";
                operation.Description = "Adds a new business user to the document inventory.";
                return operation;
            });

        app.MapPatch("/v1/document/user/{userId:guid}", async (
                [FromBody] PatchUserRequest businessUser,
                IMediator mediator,
                Guid userId) =>
            {
                using var cts = new CancellationTokenSource(timeout);
                await mediator.Send(new UpdateBusinessUserCommand(userId, businessUser.Name), cts.Token);

                return Results.NoContent();
            })
            .DisableAntiforgery()
            .WithName("updateUser")
            .WithTags(TagBusinessUsers)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Update a business user";
                operation.Description = "Updates the information of an existing business user.";

                return operation;
            });
    }
}