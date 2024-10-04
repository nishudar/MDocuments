using Common.DomainEvents;
using Documents.Api.Models;
using Documents.Application.Interfaces;
using Documents.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Documents.Api;

public static class UserEndpoints
{
    private const string TagBusinessUsers = "Business Users";

    public static void MapUserEndpoints(this IEndpointRouteBuilder app, TimeSpan timeout)
    {
        app.MapGet("/v1/document/user", async (
                IDocumentInventoryRepository repository) =>
            {
                using var cts = new CancellationTokenSource(timeout);
                var documentInventory = await repository.GetDocumentInventory(cts.Token);
                var users = documentInventory.GetUsers();

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
                IDocumentInventoryRepository repository,
                IDomainEventDispatcher dispatcher) =>
            {
                using var cts = new CancellationTokenSource(timeout);
                var documentInventory = await repository.GetDocumentInventory(cts.Token);
                var id = Guid.NewGuid();
                documentInventory.SetBusinessUser(new BusinessUser
                {
                    Id = id,
                    Name = businessUser.Name
                });
                await dispatcher.DispatchEvents(documentInventory.BusinessEvents, cts.Token);

                return Results.Ok(new IdResponse(id));
            })
            .DisableAntiforgery()
            .WithName("addUser")
            .WithTags(TagBusinessUsers)
            .Produces<IdResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Add a business user";
                operation.Description = "Adds a new business user to the document inventory.";
                return operation;
            });

        app.MapPatch("/v1/document/user/{id:guid}", async (
                [FromBody] PatchUserRequest businessUser,
                IDocumentInventoryRepository repository,
                IDomainEventDispatcher dispatcher,
                Guid id) =>
            {
                using var cts = new CancellationTokenSource(timeout);
                var documentInventory = await repository.GetDocumentInventory(cts.Token);
                documentInventory.SetBusinessUser(new BusinessUser
                {
                    Id = id,
                    Name = businessUser.Name
                });
                await dispatcher.DispatchEvents(documentInventory.BusinessEvents, cts.Token);

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