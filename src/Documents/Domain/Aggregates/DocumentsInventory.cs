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
    private List<DocumentType> AllowedDocumentTypes { get; } = documentTypes.ToList();
    private List<Process> Processes { get; } = processes.ToList();

    public Process StartProcess(Guid operatorId, Guid customerId)
    {
        var user = Users.Find(u => u.Id == operatorId);
        var customer = Users.Find(u => u.Id == customerId);
        var existingProcess = FindProcess(operatorId, customerId, ProcessStatus.Started) 
                              ?? FindProcess(operatorId, customerId);
        
        if (user is null)
            throw new UserNotExistsException(operatorId);
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
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            OperatorUserId = operatorId,
            Documents = [],
            AllowedDocumentTypes = AllowedDocumentTypes
                .Select(d => new ProcessDocumentType
                {
                    ProcessId = Guid.NewGuid(),
                    DocumentTypeId = d.Id,
                    TypeName = d.TypeName,
                    IsRequired = d.IsRequired,
                    MultipleAllowed = d.MultipleAllowed
                }).ToList(),
            Status = ProcessStatus.Started
        };

        Processes.Add(newProcess);
        PublishBusinessEvent(new ProcessChangedStatusEvent {Process = newProcess});

        return newProcess;
    }

    public void FinishProcess(Guid operatorId, Guid customerId)
    {
        var user = Users.Find(u => u.Id == operatorId);
        var customer = Users.Find(u => u.Id == customerId);
        var existingProcess = FindProcess(operatorId, customerId, ProcessStatus.Started) 
                              ?? FindProcess(operatorId, customerId);

        if (user is null)
            throw new UserNotExistsException(operatorId);
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
        existingProcess.Status = ProcessStatus.Finished;
        PublishBusinessEvent(new ProcessChangedStatusEvent {Process = existingProcess});
    }

    private Process? FindProcess(Guid operatorUserId, Guid customerId)
    {
        return Processes.Find(process =>
            process.OperatorUserId == operatorUserId
            && process.CustomerId == customerId);
    }
    
    private Process? FindProcess(Guid operatorUserId, Guid customerId, string status)
    {
        return Processes.Find(process =>
            process.OperatorUserId == operatorUserId 
            && process.CustomerId == customerId
            && process.Status == status);
    }

    public void AbandonProcess(Guid operatorId, Guid customerId)
    {
        var user = Users.Find(u => u.Id == operatorId);
        var customer = Users.Find(u => u.Id == customerId);
        if (user is null)
            throw new UserNotExistsException(operatorId);
        if (customer is null)
            throw new CustomerDoesNotExistException(customerId);
        
        var process = FindProcess(operatorId, customerId, ProcessStatus.Started) 
                              ?? FindProcess(operatorId, customerId);

        if (process is null)
            throw new ProcessNotFoundException(customerId, user.Id);
        
        process.Status = ProcessStatus.Abandoned;
        PublishBusinessEvent(new ProcessChangedStatusEvent {Process = process});
    }

    public void AddUser(User user)
    {
        var existingUser = Users.Find(u => u.Id == user.Id);
        if (existingUser is null)
        {
            var userToAdd = user.DeepClone(); 
            Users.Add(userToAdd);
            PublishBusinessEvent(new UserAddedEvent {User = user});
        }
        else
            throw new UserAlreadyExistsException(user.Id);
    }
    
    public User UpdateUser(Guid userId, string name)
    {
        var existingUser = Users.Find(u => u.Id == userId);
        if(existingUser is null)
            throw new UserNotExistsException(userId);
        existingUser.SetName(name);
        PublishBusinessEvent(new UserAddedEvent() {User = existingUser});
            
        return existingUser;
    }
    
    public void ValidateDocument(Document document)
    {
        var existingProcess = Processes.Find(process =>
            process.OperatorUserId == document.UserId && process.CustomerId == document.CustomerId);
        if (existingProcess is null)
            throw new ProcessNotFoundException(document.CustomerId, document.UserId);
        existingProcess.ValidateDocument(document);
    }

    public void AddDocument(Document document)
    {
        var process = Processes.Find(process =>
            process.OperatorUserId == document.UserId && process.CustomerId == document.CustomerId);
        if (process is null)
            throw new ProcessNotFoundException(document.CustomerId, document.UserId);
        process.AddDocument(document.DeepClone());
        PublishBusinessEvent(new DocumentAddedEvent {Document = document, Process = process});
    }

    public ProcessReport GetReport(Guid processId)
    {
        var process = Processes.Find(process => process.Id == processId);
        if (process is null)
            throw new NotFoundException("process", processId);
        var user = Users.Find(user => user.Id == process.OperatorUserId);
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
        PublishBusinessEvent(new ProcessReportGeneratedEvent {ProcessId = processId, ProcessReport = report});

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