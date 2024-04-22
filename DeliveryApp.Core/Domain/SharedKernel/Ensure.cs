#nullable enable
using DeliveryApp.Core.Domain.SharedKernel.Exceptions;

namespace DeliveryApp.Core.Domain.SharedKernel;

public static class Ensure
{
    public static T NotNull<T>(T? item)
        => item ?? throw new ValidationException(nameof(item), "Value cannot be null");
    
    public static long GreaterThanZero(long item)
        => item > 0 ? item : throw new ValidationException(nameof(item), "Value must be greater than zero");
}