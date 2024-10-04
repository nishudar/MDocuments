using Common.Abstracts;

namespace Documents.Domain.Exceptions;

public sealed class UserDoesNotExistException() : BusinessException("User does not exist")
{
    public UserDoesNotExistException(Guid userId) : this()
    {
        Data[nameof(userId)] = userId; 
    }
}

public sealed class CustomerDoesNotExistException() : BusinessException("Customer does not exist")
{
    public CustomerDoesNotExistException(Guid customerId) : this()
    {
        Data[nameof(customerId)] = customerId;
    }
}

public sealed class ProcessForDocumentNotFoundException() : BusinessException("Process for document not found")
{
    public ProcessForDocumentNotFoundException(Guid customerId, Guid userId) : this()
    {
        Data[nameof(customerId)] = customerId;
        Data[nameof(userId)] = userId;
    }
}

public sealed class ProcessAlreadyStartedException() : BusinessException("Cannot start process as it is already started")
{
    public ProcessAlreadyStartedException(Guid customerId, Guid userId, Guid processId) : this()
    {
        Data[nameof(customerId)] = customerId;
        Data[nameof(userId)] = userId;
        Data[nameof(processId)] = processId;
    }
}

public sealed class ProcessCannotChangeStatus(string reason) : BusinessException($"Cannot finish process: {reason}")
{
    public ProcessCannotChangeStatus(string reason, Guid processId) : this(reason)
    {
        Data[nameof(processId)] = processId;
    }
}


public sealed class InvalidUserIdException() : BusinessException($"Invalid user id for document")
{
    public InvalidUserIdException(Guid userId, Guid documentId) : this()
    {
        Data[nameof(userId)] = userId;
        Data[nameof(documentId)] = documentId;
    }
}

public sealed class InvalidCustomerIdException() : BusinessException($"Invalid user id for document")
{
    public InvalidCustomerIdException(Guid customerId, Guid documentId) : this()
    {
        Data[nameof(customerId)] = customerId;
        Data[nameof(documentId)] = documentId;
    }
}

public sealed class DocumentTypeNotAllowedException() : BusinessException($"Document type is not allowed")
{
    public DocumentTypeNotAllowedException(string documentType, Guid processId) : this()
    {
        Data[nameof(documentType)] = documentType;
        Data[nameof(processId)] = processId;
    }
}

public sealed class DuplicatedDocumentTypeNotAllowedException() : BusinessException($"Duplicated document type is not allowed")
{
    public DuplicatedDocumentTypeNotAllowedException(string documentType, Guid processId) : this()
    {
        Data[nameof(documentType)] = documentType;
        Data[nameof(processId)] = processId;
    }
}
