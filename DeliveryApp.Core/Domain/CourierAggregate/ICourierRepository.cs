﻿using Primitives;

namespace DeliveryApp.Core.Domain.CourierAggregate;

public interface ICourierRepository : IRepository<Courier>
{
    Courier Add(Courier courier);            // Добавить курьера
    void Update(Courier courier);            // Обновить курьера
    Task<Courier?> Get(Guid courierId); //Получить курьера
}