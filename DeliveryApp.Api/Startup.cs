using System.Reflection;
using Api.Filters;
using Api.Formatters;
using Api.OpenApi;
using DeliveryApp.Api.Adapters.Kafka.BasketConfirmed;
using DeliveryApp.Core.Application.DomainEventHandlers;
using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Core.DomainServices.Dispatch;
using DeliveryApp.Core.Ports;
using DeliveryApp.Infrastructure;
using DeliveryApp.Infrastructure.Adapters.Grpc.GeoService;
using DeliveryApp.Infrastructure.Adapters.Kafka.OrderStatusChanged;
using DeliveryApp.Infrastructure.Adapters.Postgres.Couriers;
using DeliveryApp.Infrastructure.Adapters.Postgres.Orders;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
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
            
            // HTTP Handlers
            services.AddControllers(options =>
                    {
                        options.InputFormatters.Insert(0, new InputFormatterStream());
                    })
                    .AddNewtonsoftJson(options =>
                    {
                        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                        options.SerializerSettings.Converters.Add(new StringEnumConverter
                        {
                            NamingStrategy = new CamelCaseNamingStrategy()
                        });
                    });
            
            // Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("1.0.0", new OpenApiInfo
                {
                    Title = "Delivery Service",
                    Description = "Отвечает за учет курьеров, деспетчеризацию доставкуов, доставку",
                    Contact = new OpenApiContact
                    {
                        Name = "Kirill Vetchinkin",
                        Url = new Uri("https://microarch.ru"),
                        Email = "info@microarch.ru"
                    }
                });
                options.CustomSchemaIds(type => type.FriendlyId(true));
                options.IncludeXmlComments($"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{Assembly.GetEntryAssembly()!.GetName().Name}.xml");
                options.DocumentFilter<BasePathFilter>("");
                options.OperationFilter<GeneratePathParamsValidationFilter>();
            });
            services.AddSwaggerGenNewtonsoftSupport();
            
            // Configuration
            services.Configure<Settings>(options => Configuration.Bind(options));
            var connectionString = Configuration["CONNECTION_STRING"];
            var geoServiceGrpcHost = Configuration["GEO_SERVICE_GRPC_HOST"];
            var messageBrokerHost = Configuration["MESSAGE_BROKER_HOST"];
            
            // Geo
            services.AddTransient<IGeoClient>(x => new Client(geoServiceGrpcHost!));

            // Message Broker
            services.AddHostedService<ConsumerService>(
                x => new ConsumerService(x, messageBrokerHost!));
            
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
            services.AddTransient<IBusProducer>(x=> new Producer(messageBrokerHost!));
            
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
                IRequestHandler<Queries.Couriers.GetAllCouriers.Query, Queries.Couriers.GetAllCouriers.Response>, 
                Queries.Couriers.GetAllCouriers.Handler>();
            
            // Domain Event Handlers
            services.AddTransient<INotificationHandler<OrderCreated>, OrderCreatedDomainEventHandler>();
            services.AddTransient<INotificationHandler<OrderAssignedToCourier>, OrderAssignedDomainEventHandler>();
            services.AddTransient<INotificationHandler<OrderCompleted>, OrderCompletedDomainEventHandler>();
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
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseSwagger(c =>
               {
                   c.RouteTemplate = "openapi/{documentName}/openapi.json";
               })
               .UseSwaggerUI(options =>
               {
                   options.RoutePrefix = "openapi";
                   options.SwaggerEndpoint("/openapi/1.0.0/openapi.json", "Swagger Delivery Service");
                   options.RoutePrefix = string.Empty;
                   options.SwaggerEndpoint("/openapi-original.json", "Swagger Delivery Service");
               });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}