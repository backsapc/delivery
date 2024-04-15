﻿namespace DeliveryApp.Core.Domain.SharedKernel;

public sealed class Weight : ValueObject
{
    public decimal Value { get; }
    
    private Weight(decimal value)
    {
        Value = value;
    }

    public static Weight Of(decimal value)
    {
        if (value <= 0) 
            throw new ValidationException(nameof(value), "Must be greater than 0");

        return new Weight(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}