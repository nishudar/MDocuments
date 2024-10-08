﻿using Common.DomainEvents;
using Documents.Application.Interfaces;
using Documents.Domain.Exceptions;
using Documents.Domain.ValueObjects;
using Documents.Infrastructure;
using MediatR;

namespace Documents.Application.Queries;

internal record TrackProcessQuery(Guid ProcessId) : IRequest<ProcessReport>;

internal class TrackProcessHandler(IDomainEventDispatcher dispatcher, IDocumentsUnitOfWork unitOfWork)
    : IRequestHandler<TrackProcessQuery, ProcessReport>
{
    public async Task<ProcessReport> Handle(TrackProcessQuery request, CancellationToken cancellationToken)
    {
        var documentInventory = await unitOfWork.GetDocumentInventory(cancellationToken);
        var report = documentInventory.GetReport(request.ProcessId);
        if (report is null)
            throw new NotFoundException("process", request.ProcessId);
        await dispatcher.DispatchEvents(documentInventory.BusinessEvents, cancellationToken);

        return report;
    }
}