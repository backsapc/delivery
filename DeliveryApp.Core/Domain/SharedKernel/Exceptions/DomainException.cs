﻿namespace DeliveryApp.Core.Domain.SharedKernel.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}