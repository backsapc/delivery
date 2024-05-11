using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.DomainServices.Dispatch;
using DeliveryApp.Infrastructure;
using DeliveryApp.Infrastructure.Adapters.Postgres.Couriers;
using DeliveryApp.Infrastructure.Adapters.Postgres.Orders;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Primitives;

namespace DeliveryApp.Api
{
    using Commands = Core.Application.Commands;
    using Queries = Core.Application.Queries;
    
    public class Startup
    {
        public Startup()
        {
            var builder = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddEnvironmentVariables();
            var configuration = builder.Build();
            Configuration = configuration;
        }

        /// <summary>
        /// Конфигурация
        /// </summary>
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Health Checks
            services.AddHealthChecks();

            // Cors
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                        policy.AllowAnyOrigin(); // Не делайте так в проде!
                    });
            });
            
            // Configuration
            services.Configure<Settings>(options => Configuration.Bind(options));
            var connectionString = Configuration["CONNECTION_STRING"];
            var geoServiceGrpcHost = Configuration["GEO_SERVICE_GRPC_HOST"];
            var messageBrokerHost = Configuration["MESSAGE_BROKER_HOST"];
            
            // БД 
            services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseNpgsql(connectionString,
                                      npgsqlOptionsAction: sqlOptions =>
                                      {
                                          sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure");
                                      });
                    options.EnableSensitiveDataLogging();
                }
            );
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddSingleton<Queries.GetDbConnection>(_ => () => new NpgsqlConnection(connectionString));
            
            // Ports & Adapters
            services.AddTransient<ICourierRepository, CourierRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            
            // Domain Services
            services.AddTransient<IDispatchService, DispatchService>();
            
            // MediatR 
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Startup>());

            // Commands
            services.AddTransient<IRequestHandler<Commands.Orders.CreateOrder.Command, bool>,
                Commands.Orders.CreateOrder.Handler>();
            services.AddTransient<IRequestHandler<Commands.Couriers.MoveCouriers.Command, bool>,
                Commands.Couriers.MoveCouriers.Handler>();
            services.AddTransient<IRequestHandler<Commands.Couriers.AssignOrder.Command, bool>,
                Commands.Couriers.AssignOrder.Handler>();
            services.AddTransient<IRequestHandler<Commands.Couriers.GetOnTheLine.Command, bool>,
                Commands.Couriers.GetOnTheLine.Handler>();
            services.AddTransient<IRequestHandler<Commands.Couriers.GetOffTheLine.Command, bool>,
                Commands.Couriers.GetOffTheLine.Handler>();

            // Queries
            services.AddTransient<
                IRequestHandler<Queries.Orders.GetActiveOrders.Query, Queries.Orders.GetActiveOrders.Response>, 
                Queries.Orders.GetActiveOrders.Handler>();
            
            services.AddTransient<
                IRequestHandler<Queries.Couriers.GetActiveCouriers.Query, Queries.Couriers.GetActiveCouriers.Response>, 
                Queries.Couriers.GetActiveCouriers.Handler>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHealthChecks("/health");
            app.UseRouting();
        }
    }
}