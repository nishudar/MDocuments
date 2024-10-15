using Common.Abstracts;
using Documents.Domain.Entities;
using Documents.Domain.ValueObjects;

namespace Documents.Domain.Aggregates;

internal interface IDocumentsAggregateRoot : IAggregateRoot
{
    Process StartProcess(Guid operatorId, Guid customerId);
    void FinishProcess(Guid operatorId, Guid customerId);
    void AbandonProcess(Guid operatorId, Guid customerId);
    public void AddUser(User user);
    public User UpdateUser(Guid userId, string name);
    void ValidateDocument(Document document);
    void AddDocument(Document document);
    ProcessReport? GetReport(Guid processId);
    Document? GetDocument(Guid documentId);
    IEnumerable<User> GetUsers();
    IEnumerable<Process> GetProcesses();
}