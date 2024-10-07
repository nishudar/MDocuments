using Common.Abstracts;

namespace Documents.Domain.Exceptions;

public sealed class CustomerDoesNotExistException() : BusinessException("Customer does not exist")
{
    public CustomerDoesNotExistException(Guid customerId) : this()
    {
        Data[nameof(customerId)] = customerId;
    }
}