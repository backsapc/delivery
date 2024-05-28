using MediatR;
using Primitives;

namespace DeliveryApp.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly IMediator _mediator;
    private readonly ApplicationDbContext _dbContext;

    public UnitOfWork(IMediator mediator, ApplicationDbContext dbContext)
    {
        _mediator = mediator;
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }
    
    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await DispatchDomainEventsAsync();
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
    
    private async Task DispatchDomainEventsAsync()
    {
        var domainEntities = _dbContext.ChangeTracker
                                       .Entries<AggregateRoot>()
                                       .Where(x => x.Entity.GetDomainEvents().Any())
                                       .ToArray();

        var domainEvents = domainEntities
                           .SelectMany(x => x.Entity.GetDomainEvents())
                           .ToList();

        domainEntities.ToList()
                      .ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (DomainEvent domainEvent in domainEvents)
            if(_mediator is not null)
                await _mediator.Publish(domainEvent);
    }
}