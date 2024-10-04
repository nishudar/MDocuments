using Common.DomainEvents;
using Documents.Api.Models;
using Documents.Application.Interfaces;
using Documents.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Documents.Api;

public static class CustomersEndpoints
{
    private const string TagCustomers = "Customers";
    
    public static void MapConsumerEndpoints(this IEndpointRouteBuilder app, TimeSpan timeout)
    {
        app.MapGet("/v1/document/customer", async (
                IDocumentInventoryRepository repository) =>
            {
                using var cts = new CancellationTokenSource(timeout);
                var documentInventory = await repository.GetDocumentInventory(cts.Token);
                var customers = documentInventory.GetCustomers();

                return Results.Ok(customers);
            })
            .DisableAntiforgery()
            .WithName("getCustomers")
            .WithTags(TagCustomers)
            .Produces<IEnumerable<Customer>>()
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Get list of customers";
                operation.Description = "Get list of customers and assigned business users";
                return operation;
            });
        
        
        app.MapPost("/v1/documents/users/{userId:guid}/customers", async (
                [FromBody] AssignCustomerRequest customer,
                [FromRoute] Guid userId,
                IDocumentInventoryRepository repository,
                IDomainEventDispatcher dispatcher
            ) =>
            {
                using var cts = new CancellationTokenSource(timeout);
                var documentInventory = await repository.GetDocumentInventory(cts.Token);
                var id = Guid.NewGuid();
                documentInventory.AssignCustomer(new Customer
                {
                    Id = id,
                    Name = customer.Name,
                    AssignedUserId = userId
                });
                await dispatcher.DispatchEvents(documentInventory.BusinessEvents, cts.Token);
                
                return Results.Ok(new IdResponse(id));
            })
            .DisableAntiforgery()
            .WithName("assignCustomer")
            .WithTags(TagCustomers)
            .Produces<IdResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithOpenApi(operation =>
            {
                operation.Summary = "Assign a customer to a user";
                operation.Description = "Assigns a customer to a business user.";
                var userIdParam = operation.Parameters.First(p => p.Name == "userId");
                userIdParam.Description = "The unique identifier of the business user.";
                return operation;
            });

    }
}



