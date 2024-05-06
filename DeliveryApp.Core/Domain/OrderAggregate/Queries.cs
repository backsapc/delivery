namespace DeliveryApp.Core.Domain.OrderAggregate;

public delegate Task<IReadOnlyCollection<Guid>> GetAllNotAssignedOrders(); //Получить все неназначенные заказы

public delegate Task<IReadOnlyCollection<Guid>> GetAllAssignedOrders(); //Получить все назначенные заказы