namespace Common.Abstracts;

public abstract class BusinessException(string message)
    : Exception(message, null);