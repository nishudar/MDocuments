using MediatR;
using Microsoft.AspNetCore.Mvc;
using Users.Api.Models;
using Users.Application.Commands;
using Users.Application.Queries;
using Users.Domain.Entities;
using Users.Domain.Enums;

namespace Users.Api;

internal static class UserEndpoints
{
    private const string TagUsers = "Users";

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
            .WithTags(TagUsers)
            .Produces<IEnumerable<BusinessUser>>()
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Get users";
                operation.Description = $"Get list of users";
                return operation;
            });

        app.MapPost("/v1/user", async (
                [FromBody] AddUserModel businessUser,
                IMediator mediator) =>
            {
                using var cts = new CancellationTokenSource(timeout);
                var user = await mediator.Send(new AddBusinessUserCommand(businessUser.Name, businessUser.Role), cts.Token);

                return Results.Ok(new IdResponse(user.Id));
            })
            .DisableAntiforgery()
            .WithName("addUser")
            .WithTags(TagUsers)
            .Produces<IdResponse>()
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation =>
            {
                operation.Summary = $"Add a user. The role represent user capabilities.  Allowed values: {string.Join(", ", UserRole.Roles)}) ";
                operation.Description = "Adds a new user to the document inventory.";
                return operation;
            });

        app.MapPatch("/v1/user/{userId:guid}", async (
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
            .WithTags(TagUsers)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Update a user";
                operation.Description = "Updates the information of an existing  user.";

                return operation;
            });
    }
}