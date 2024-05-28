using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.Domain.SharedKernel;
using DeliveryApp.Infrastructure;
using DeliveryApp.Infrastructure.Adapters.Postgres.Orders;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests.Repositories;

public class OrderRepositoryShould : IAsyncLifetime
{
    private ApplicationDbContext _context;
    private readonly Location _location;
    private readonly Weight _weight;

    /// <summary>
    /// Настройка Postgres из библиотеки TestContainers
    /// </summary>
    /// <remarks>По сути это Docker контейнер с Postgres</remarks>
    private readonly PostgreSqlContainer _postgreSqlContainer =
        new PostgreSqlBuilder()
            .WithImage("postgres:14.7")
            .WithDatabase("order")
            .WithUsername("username")
            .WithPassword("secret")
            .WithCleanUp(true)
            .Build();

    /// <summary>
    /// Ctr
    /// </summary>
    /// <remarks>Вызывается один раз перед всеми тестами в рамках этого класса</remarks>
    public OrderRepositoryShould()
    {
        _weight = Weight.Of(2);
        _location = Location.Of(1, 1).Value;
    }

    /// <summary>
    /// Инициализируем окружение
    /// </summary>
    /// <remarks>Вызывается перед каждым тестом</remarks>
    public async Task InitializeAsync()
    {
        //Стартуем БД (библиотека TestContainers запускает Docker контейнер с Postgres)
        await _postgreSqlContainer.StartAsync();

        //Накатываем миграции и справочники
        var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseNpgsql(
            _postgreSqlContainer.GetConnectionString(),
            npgsqlOptionsAction: sqlOptions =>
            {
                sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure");
            }).Options;
        _context = new ApplicationDbContext(contextOptions);
        await _context.Database.MigrateAsync();
    }

    /// <summary>
    /// Уничтожаем окружение
    /// </summary>
    /// <remarks>Вызывается после каждого теста</remarks>
    public async Task DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync().AsTask();
    }

    [Fact]
    public async void CanAddOrder()
    {
        //Arrange
        var orderId = Guid.NewGuid();
        var order = Order.Create(orderId, _location, _weight);

        //Act
        var orderRepository = new OrderRepository(_context);
        var unitOfWork = new UnitOfWork(null, _context);
            
        orderRepository.Add(order);
        await unitOfWork.SaveEntitiesAsync();

        //Assert
        var orderFromDb = await orderRepository.Get(order.Id);
        order.Should().BeEquivalentTo(orderFromDb);
    }

    [Fact]
    public async void CanUpdateOrder()
    {
        //Arrange
        var courier = Courier.Create(Guid.NewGuid(), "Иван", Transport.Pedestrian);

        var orderId = Guid.NewGuid();
        var order = Order.Create(orderId, _location, _weight);

        var orderRepository = new OrderRepository(_context);
        orderRepository.Add(order);
            
        var unitOfWork = new UnitOfWork(null, _context);
        await unitOfWork.SaveEntitiesAsync();

        //Act
        order.AssignTo(courier.Id);
        orderRepository.Update(order);
        await unitOfWork.SaveEntitiesAsync();

        //Assert
        var orderFromDb = await orderRepository.Get(order.Id);
        order.Should().BeEquivalentTo(orderFromDb);
    }

    [Fact]
    public async void CanGetById()
    {
        //Arrange
        var orderId = Guid.NewGuid();
        var order = Order.Create(orderId, _location, _weight);

        //Act
        var orderRepository = new OrderRepository(_context);
        orderRepository.Add(order);
            
        var unitOfWork = new UnitOfWork(null, _context);
        await unitOfWork.SaveEntitiesAsync();

        //Assert
        var orderFromDb = await orderRepository.Get(order.Id);
        order.Should().BeEquivalentTo(orderFromDb);
    }

    // [Fact]
    // public async void CanGetAllActive()
    // {
    //     //Arrange
    //     var courier = Courier.Create(Guid.NewGuid(), "Иван", Transport.Pedestrian);
    //
    //     var order1Id = Guid.NewGuid();
    //     var order1 = Order.Create(order1Id, _location, _weight);
    //     order1.AssignTo(courier.Id);
    //
    //     var order2Id = Guid.NewGuid();
    //     var order2 = Order.Create(order2Id, _location, _weight);
    //
    //     var orderRepository = new OrderRepository(_context);
    //     orderRepository.Add(order1);
    //     orderRepository.Add(order2);
    //     
    //     var unitOfWork = new UnitOfWork(_context);
    //     await unitOfWork.SaveEntitiesAsync();
    //
    //     //Act
    //     var activeOrdersFromDb = orderRepository.GetAllNotAssigned();
    //
    //     //Assert
    //     var ordersFromDb = activeOrdersFromDb.ToList();
    //     ordersFromDb.Should().NotBeEmpty();
    //     ordersFromDb.Count().Should().Be(1);
    //     ordersFromDb.First().Should().BeEquivalentTo(order2);
    // }
}