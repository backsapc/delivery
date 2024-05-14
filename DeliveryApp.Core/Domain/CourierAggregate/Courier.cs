using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.SharedKernel;
using Primitives;

namespace DeliveryApp.Core.Domain.CourierAggregate;

public sealed class Courier : Aggregate<Guid>
{
    public string Name { get; private set; }
    public Transport Transport { get; private set; }
    public Location CourierLocation { get; private set; }
    public CourierStatus Status { get; private set; }
    public Guid? CurrentOrderId { get; private set; }

    private Courier() { }
    
    private Courier(
        Guid courierId, 
        string name, 
        Transport transport, 
        Location courierLocation, 
        CourierStatus status)
        : base(courierId)
    {
        Name = name;
        Transport = transport;
        CourierLocation = courierLocation;
        Status = status;
    }

    public static Courier Create(Guid courierId,
                                 string name,
                                 Transport transport)
    {
        return new Courier(
            courierId,
            Ensure.NotNull(name),
            Ensure.NotNull(transport),
            Location.Of(1, 1).Value,
            CourierStatus.NotAvailable);
    }

    public void GetOnTheLine()
    {
        if (Status == CourierStatus.Busy)
            throw Exceptions.CourierIsAlreadyBusyToGetOnTheLine(Id);

        Status = CourierStatus.Ready;
    }

    public void GetOffTheLine()
    {
        if (Status == CourierStatus.Busy)
            throw Exceptions.CourierIsAlreadyBusyToGetOffTheLine(Id);

        Status = CourierStatus.NotAvailable;
    }

    public void AcceptOrder(Guid orderId)
    {
        if (CurrentOrderId == orderId) return;

        if (Status == CourierStatus.Busy)
            throw Exceptions.CourierIsAlreadyBusyToAcceptTheOrder(Id);

        if (Status == CourierStatus.NotAvailable)
            throw Exceptions.CourierIsNotAvailableToAcceptTheOrder(Id);

        Status = CourierStatus.Busy;
        CurrentOrderId = orderId;

        RaiseDomainEvent(new OrderAcceptedByCourier
        {
            CourierId = Id,
            OrderId = orderId
        });
    }

    public long CalculateTime(Location location)
    {
        return Transport.CalculateTime(CourierLocation, location);
    }

    public void StepTowardTheOrder(Location location)
    {
        (int StepX, int StepY)[] possibleSteps =
        [
            (0, 1),
            (-1, 0), (1, 0),
            (0, -1)
        ];

        var bestLocation = CourierLocation;
        for (int i = 0; i < Transport.Speed && bestLocation != location; i++)
        {
            bestLocation =
                possibleSteps
                    .Select(x => Location.Of(
                                bestLocation.PositionX + x.StepX,
                                bestLocation.PositionY + x.StepY))
                    .Where(x => x.IsSuccess)
                    .Select(x => x.Value)
                    .OrderBy(location.DistanceTo)
                    .First();
        }

        CourierLocation = bestLocation;
        
        if (location != CourierLocation)
        {
            return;
        }

        if(CurrentOrderId.HasValue) 
        {
            RaiseDomainEvent(new CourierDeliveredOrder
            {
                OrderId = CurrentOrderId.Value,
                CourierId = Id
            });
            
            CurrentOrderId = null;
            Status = CourierStatus.Ready;
        }
        else
        {
            // Courier is on the location, but he has no order...
            // Should not be here.
        }
    }

    public bool CanHandle(Weight weight) => Transport.CanHandle(weight);

    public Location CurrentLocation() => CourierLocation;
    public CourierStatus CurrentStatus() => Status;
}

public sealed class Transport : Entity<int>
{
    public static IReadOnlyCollection<Transport> List() => [
        Pedestrian, Bicycle, Scooter, Car
    ];
    
    public static readonly Transport Pedestrian = Create(1, "pedestrian", 1, 1);
    public static readonly Transport Bicycle = Create(2, "bicycle", 4, 2);
    public static readonly Transport Scooter = Create(3, "scooter", 6, 3);
    public static readonly Transport Car = Create(4, "car", 8, 4);

    private Transport(int id, string name, long capacity, long speed) : base(id)
    {
        Name = name;
        Capacity = capacity;
        Speed = speed;
    }

    public string Name { get; }
    public long Capacity { get; }
    public long Speed { get; }

    private static Transport Create(int id, string name, long capacity, long speed)
        => new(
            id,
            Ensure.NotNull(name),
            Ensure.GreaterThanZero(capacity),
            Ensure.GreaterThanZero(speed));

    public bool CanHandle(Weight weight) => weight.Value <= Capacity;

    public long CalculateTime(Location first, Location second)
    {
        var steps = first.DistanceTo(second) / (decimal) Speed;
        return (long) Math.Round(steps, MidpointRounding.ToPositiveInfinity);
    }
}