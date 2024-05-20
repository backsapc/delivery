using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Core.Domain.SharedKernel.Exceptions;

namespace DeliveryApp.Core.Ports;

public interface IGeoClient
{
    public Task<Result<Location, DomainException>> Location(string address, CancellationToken cancellationToken);
}