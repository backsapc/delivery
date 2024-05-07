namespace DeliveryApp.Core.Domain.CourierAggregate;

public delegate Task<IReadOnlyCollection<Guid>> GetAllReadyCouriers(); // Получить всех свободных курьеров

public delegate Task<IReadOnlyCollection<Guid>> GetAllBusyCouriers(); // Получить всех занятых курьеров