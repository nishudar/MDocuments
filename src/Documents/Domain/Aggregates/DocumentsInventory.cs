using Common.Abstracts;
using Documents.Domain.Entities;
using Documents.Domain.Enums;
using Documents.Domain.Events;
using Documents.Domain.Exceptions;
using Documents.Domain.ValueObjects;
using Force.DeepCloner;

namespace Documents.Domain.Aggregates;

internal class DocumentsInventory(
    ICollection<User> users,
    ICollection<DocumentType> documentTypes,
    ICollection<Process> processes)
    : Aggregate, IDocumentsInventory
{
    private List<User> Users { get; } = users.ToList();
    private List<Document> Documents { get; set; } = processes.SelectMany(process => process.Documents).ToList();
    private List<DocumentType> AllowedDocumentTypes { get; } = documentTypes.ToList();
    private List<Process> Processes { get; } = processes.ToList();

    public Process StartProcess(Guid operatorId, Guid customerId)
    {
        var user = Users.Find(u => u.Id == operatorId);
        var customer = Users.Find(u => u.Id == customerId);
        var existingProcess = Processes.Find(process =>
            process.BusinessUserId == operatorId && process.CustomerId == customerId);
        
        if (user is null)
            throw new UserDoesNotExistException(operatorId);
        if (customer is null)
            throw new CustomerDoesNotExistException(customerId);
        
        if(user.Role is not UserRole.Operator)
            throw new NotInRoleException(user.Id, user.Role, UserRole.Operator);
        if(customer.Role is not UserRole.Customer)
            throw new NotInRoleException(user.Id, user.Role, UserRole.Customer);
        
        if (existingProcess is not null && existingProcess.Status == ProcessStatus.Started)
            throw new ProcessAlreadyStartedException(customerId, operatorId, existingProcess.Id);

        var newProcess = new Process
        {
            CustomerId = customerId,
            BusinessUserId = operatorId,
            Id = Guid.NewGuid(),
            Documents = [],
            AllowedDocumentTypes = AllowedDocumentTypes.ToArray(),
        };
        newProcess.SetStatus(ProcessStatus.Started);

        Processes.Add(newProcess);
        AddBusinessEvent(new ProcessChangedStatusEvent {Process = newProcess});

        return newProcess;
    }

    public void FinishProcess(Guid businessUserId, Guid customerId)
    {
        var user = Users.Find(u => u.Id == businessUserId);
        var customer = Users.Find(u => u.Id == customerId);
        var existingProcess = Processes.Find(process =>
            process.BusinessUserId == businessUserId && process.CustomerId == customerId);
        
        if (user is null)
            throw new UserDoesNotExistException(businessUserId);
        if (customer is null)
            throw new CustomerDoesNotExistException(customerId);
        
        if(user.Role is not UserRole.Operator)
            throw new NotInRoleException(user.Id, user.Role, UserRole.Operator);
        if(customer.Role is not UserRole.Customer)
            throw new NotInRoleException(user.Id, user.Role, UserRole.Customer);
        
        if (existingProcess is null)
            throw new ProcessCannotChangeStatusException("not started");
        if (existingProcess is {Status: ProcessStatus.Abandoned})
            throw new ProcessCannotChangeStatusException("abandoned", existingProcess.Id);
        if (existingProcess is {Status: ProcessStatus.Finished}) throw new ProcessCannotChangeStatusException("already finished", existingProcess.Id);

        if (!existingProcess.AllDocumentsProvided())
            throw new ProcessCannotChangeStatusException("There are missing documents");
        
        existingProcess.SetStatus(ProcessStatus.Finished);
        AddBusinessEvent(new ProcessChangedStatusEvent {Process = existingProcess});
    }

    public void AbandonProcess(Guid operatorId, Guid customerId)
    {
        var user = Users.Find(u => u.Id == operatorId);
        var customer = Users.Find(u => u.Id == customerId);
        if (user is null)
            throw new UserDoesNotExistException(operatorId);
        if (customer is null)
            throw new CustomerDoesNotExistException(customerId);
        
        var process = Processes.Find(process =>
            process.BusinessUserId == operatorId 
            && process.CustomerId == customerId
            && process.Status == ProcessStatus.Started);

        if (process is null)
            throw new ProcessNotFoundException(customerId, user.Id);

        
        process.SetStatus(ProcessStatus.Abandoned);
        AddBusinessEvent(new ProcessChangedStatusEvent {Process = process});
    }

    public void AddUser(User user)
    {
        var existingUser = Users.Find(u => u.Id == user.Id);
        if (existingUser is null)
        {
            var userToAdd = user.DeepClone(); 
            Users.Add(userToAdd);
            AddBusinessEvent(new UserAddedEvent {User = user});
        }
        else
            throw new UserAlreadyExistsException(user.Id);
    }
    
    public User UpdateUser(Guid userId, string name)
    {
        var existingUser = Users.Find(u => u.Id == userId);
        if(existingUser is null)
            throw new UserDoesNotExistException(userId);
        existingUser.SetName(name);
        AddBusinessEvent(new UserAddedEvent() {User = existingUser});
            
        return existingUser;
    }
    
    public void ValidateDocument(Document document)
    {
        var existingProcess = Processes.Find(process =>
            process.BusinessUserId == document.UserId && process.CustomerId == document.CustomerId);
        if (existingProcess is null)
            throw new ProcessNotFoundException(document.CustomerId, document.UserId);
        existingProcess.ValidateDocument(document);
    }

    public void AddDocument(Document document)
    {
        var process = Processes.Find(process =>
            process.BusinessUserId == document.UserId && process.CustomerId == document.CustomerId);
        if (process is null)
            throw new ProcessNotFoundException(document.CustomerId, document.UserId);
        process.AddDocument(document.DeepClone());
        AddBusinessEvent(new DocumentAddedEvent {Document = document, Process = process});
    }

    public ProcessReport GetReport(Guid processId)
    {
        var process = Processes.Find(process => process.Id == processId);
        if (process is null)
            throw new NotFoundException("process", processId);
        var user = Users.Find(user => user.Id == process.BusinessUserId);
        var customer = Users.Find(u => u.Id == process.CustomerId);
        var requiredDocumentTypes = process.AllowedDocumentTypes
            .Where(document => document.IsRequired)
            .Select(rdtn => rdtn.TypeName)
            .ToArray();
        var providedDocuments = process
            .Documents
            .Select(d => new DocumentWithType(d.Name, d.DocumenType))
            .ToArray();

        var report = new ProcessReport(processId, user?.Id, customer?.Id, user?.Name!, customer?.Name!,
            requiredDocumentTypes, providedDocuments);
        AddBusinessEvent(new ProcessReportGeneratedEvent {ProcessId = processId, ProcessReport = report});

        return report;
    }

    public Document? GetDocument(Guid documentId)
    {
        return Processes.SelectMany(process => process.Documents)
            .FirstOrDefault(d => d.Id == documentId);
    }

    public IEnumerable<User> GetUsers()
    {
        return Users.ToArray();
    }

    public IEnumerable<User> GetCustomers()
    {
        return Users.Where( user => user.Role == UserRole.Customer).ToList();
    }

    public IEnumerable<Process> GetProcesses()
    {
        return Processes.ToArray();
    }
}