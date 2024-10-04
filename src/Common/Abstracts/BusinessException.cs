namespace Common.Abstracts;

public abstract class BusinessException(string message, Exception? innerException = null) : Exception(message, innerException = null);