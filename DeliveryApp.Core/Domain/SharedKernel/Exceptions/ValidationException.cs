namespace DeliveryApp.Core.Domain.SharedKernel.Exceptions;

public class ValidationException : DomainException
{
    public string ValidatedObjectName { get; }

    public ValidationException(string validatedObjectName, string message) : base(message)
    {
        ValidatedObjectName = validatedObjectName;
    }

    public override string ToString()
    {
        return $"Validation failed for field '{ValidatedObjectName}' with message '{Message}'";
    }
}