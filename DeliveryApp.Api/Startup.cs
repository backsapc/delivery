using DeliveryApp.Core.Domain.CourierAggregate;
using DeliveryApp.Core.Domain.OrderAggregate;
using DeliveryApp.Infrastructure;
using DeliveryApp.Infrastructure.Adapters.Postgres.Couriers;
using DeliveryApp.Infrastructure.Adapters.Postgres.Orders;
using Microsoft.EntityFrameworkCore;
using Primitives;

namespace DeliveryApp.Api
{
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
            
            // Ports & Adapters
            services.AddTransient<ICourierRepository, CourierRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();
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