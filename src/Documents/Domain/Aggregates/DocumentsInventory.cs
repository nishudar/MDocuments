using Common.Abstracts;
using Documents.Domain.Entities;
using Documents.Domain.Events;
using Documents.Domain.Exceptions;
using Documents.Domain.ValueTypes;

namespace Documents.Domain.Aggregates;

public class DocumentsInventory(
    IEnumerable<BusinessUser> users,
    IEnumerable<Customer> customers,
    IEnumerable<DocumentType> documentTypes,
    IEnumerable<Process> processes)
    : Aggregate, IDocumentsInventory
{
    private List<BusinessUser> Users { get; set; } = users.ToList();
    private List<Document> Documents { get; set; } = processes.SelectMany(process => process.Documents).ToList();
    private List<Customer> Customers { get; set; } = customers.ToList();
    private List<DocumentType> AllowedDocumentTypes { get; set; } = documentTypes.ToList();
    private List<Process> Processes { get; set; } = processes.ToList();
    
    public Process StartProcess(Guid businessUserId, Guid customerId)
    {
        var user = Users.Find(u => u.Id == businessUserId);
        var customer = Customers.Find(c => c.Id == customerId);
        var existingProcess = Processes.Find(process => process.BusinessUserId == businessUserId && process.CustomerId == customerId);
        
        if(user is null)
            throw new UserDoesNotExistException(businessUserId);
        if(customer is null)
            throw new CustomerDoesNotExistException(customerId);
        if (existingProcess is {Status: ProcessStatus.Started})
            throw new ProcessAlreadyStartedException(customerId, businessUserId, existingProcess.Id);

        var newProcess = new Process
        {
            CustomerId = customerId,
            BusinessUserId = businessUserId,
            Id = Guid.NewGuid(),
            Documents = [],
            AllowedDocumentTypes = AllowedDocumentTypes.ToArray(),
        };
        
        Processes.Add(newProcess);
        BusinessEvents.Add(new ProcessChangedStatusEvent{Process = newProcess});
        
        return newProcess;
    }

    public void FinishProcess(Guid businessUserId, Guid customerId)
    {
        var user = Users.Find(u => u.Id == businessUserId);
        var customer = Customers.Find(c => c.Id == customerId);
        var existingProcess = Processes.Find(process =>
            process.BusinessUserId == businessUserId && process.CustomerId == customerId);
        if(user is null)
            throw new UserDoesNotExistException(businessUserId);
        if(customer is null)
            throw new CustomerDoesNotExistException(customerId);
        if (existingProcess is null)
            throw new ProcessCannotChangeStatus("not started");
        switch (existingProcess)
        {
            case {Status: ProcessStatus.Abandoned}:
                throw new ProcessCannotChangeStatus("abandoned", existingProcess.Id);
            case {Status: ProcessStatus.Finished}:
                throw new ProcessCannotChangeStatus("already finished", existingProcess.Id);
        }

        existingProcess.SetStatus(ProcessStatus.Finished);
        BusinessEvents.Add(new ProcessChangedStatusEvent{Process = existingProcess});
    }
    
    public void AbandonProcess(Guid businessUserId, Guid customerId)
    {
        var user = Users.Find(u => u.Id == businessUserId);
        var customer = Customers.Find(c => c.Id == customerId);
        var process = Processes.Find(process => process.BusinessUserId == businessUserId && process.CustomerId == customerId);
        
        if(user is null)
            throw new UserDoesNotExistException(businessUserId);
        if(customer is null)
            throw new CustomerDoesNotExistException(customerId);
        if (process is null)
            throw new ProcessCannotChangeStatus("not started");
        if(process.Status is ProcessStatus.Finished)
            throw new ProcessCannotChangeStatus("already finished", process.Id);
        
        process.SetStatus(ProcessStatus.Abandoned);
        BusinessEvents.Add(new ProcessChangedStatusEvent{Process = process});
    }

    public void SetBusinessUser(BusinessUser user)
    {
        var existingUser = Users.Find(u => u.Id == user.Id);
        if (existingUser is not null && existingUser.Name != user.Name)
        {
            existingUser.Set(user);
            BusinessEvents.Add(new BusinessUserUpdatedEvent {User = user});
        }
        else
        {
            Users.Add(user);
            BusinessEvents.Add(new BusinessUserAddedEvent {User = user});
        }
    }
    
    public void AssignCustomer(Customer customer)
    {
        var existingUser = Users.Find(u => u.Id == customer.AssignedUserId);
        if (existingUser is null)
            throw new UserDoesNotExistException(customer.AssignedUserId);
        var existingCustomer = Customers.Find(c => c.Id == customer.Id);
        if (existingCustomer is null)
        {
            Customers.Add(customer);
            BusinessEvents.Add(new CustomerAddedEvent{Customer = customer});
        }
        else if(existingCustomer.AssignedUserId != customer.AssignedUserId)
        {
            customer.ReassignUser(customer);
            BusinessEvents.Add(new CustomerReassignedEvent{Customer = customer});
        }
    }

    public void ValidateDocument(Document document)
    {
        var existingProcess = Processes.Find(process => process.BusinessUserId == document.UserId && process.CustomerId == document.CustomerId);
        if(existingProcess is null)
            throw new ProcessForDocumentNotFoundException(document.CustomerId, document.UserId);
        existingProcess.ValidateDocument(document);
    }
        
    public void AddDocument(Document document)
    {
        var existingProcess = Processes.Find(process => process.BusinessUserId == document.UserId && process.CustomerId == document.CustomerId);
        if(existingProcess is null)
            throw new ProcessForDocumentNotFoundException(document.CustomerId, document.UserId);
        existingProcess.AddDocument(document);
        BusinessEvents.Add(new DocumentAddedEvent {Document = document});
    }
    
    public ProcessReport? GetReport(Guid processId)
    {
        var process = Processes.Find(process => process.Id == processId);
        if (process is null)
            return null;
        var user = Users.Find(user => user.Id == process.BusinessUserId);
        var customer = Customers.Find(customer => customer.Id == process.CustomerId);
        var requiredDocumentTypes = process.AllowedDocumentTypes
            .Where(document => document.IsRequired)
            .Select(rdtn => rdtn.TypeName)
            .ToArray();
        var providedDocuments = process
            .Documents
            .Select(d => new DocumentWithType(d.Name, d.DocumenType))
            .ToArray();

        var report = new ProcessReport(processId, user?.Id, customer?.Id, user?.Name!, customer?.Name!, requiredDocumentTypes, providedDocuments);
        BusinessEvents.Add(new ProcessReportGeneratedEvent {ProcessId = processId, ProcessReport = report});
        
        return report;
    }

    public Document? GetDocument(Guid documentId) 
        =>  Processes.SelectMany(process => process.Documents)
            .FirstOrDefault(d => d.Id == documentId);

    public IEnumerable<BusinessUser> GetUsers() => Users.ToArray();
    public IEnumerable<Customer> GetCustomers() => Customers.ToArray();
    
    public IEnumerable<Process> GetProcesses() => Processes.ToArray();
}