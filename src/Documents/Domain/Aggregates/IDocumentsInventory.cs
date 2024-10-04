using Documents.Domain.Entities;
using Documents.Domain.ValueTypes;

namespace Documents.Domain.Aggregates;

public interface IDocumentsInventory
{
    Process StartProcess(Guid businessUserId, Guid customerId);
    void FinishProcess(Guid businessUserId, Guid customerId);
    void AbandonProcess(Guid businessUserId, Guid customerId);
    void SetBusinessUser(BusinessUser user);
    void AssignCustomer(Customer customer);
    void ValidateDocument(Document document);
    void AddDocument(Document document);
    ProcessReport? GetReport(Guid processId);
    Document? GetDocument(Guid documentId);
    IEnumerable<BusinessUser> GetUsers();
    IEnumerable<Customer> GetCustomers();
    IEnumerable<Process> GetProcesses();
    Guid Id { get; init; }
}