using CSharpFunctionalExtensions;

namespace Primitives
{
    public abstract class Aggregate<TKey> : Entity<TKey>, AggregateRoot where TKey : IComparable<TKey>
    {
        private readonly List<IDomainEvent> _domainEvents = new();

        protected Aggregate(TKey id) : base(id)
        {
        }

        protected Aggregate()
        {
        }

        public IReadOnlyCollection<IDomainEvent> GetDomainEvents() => _domainEvents.ToList();

        public void ClearDomainEvents() => _domainEvents.Clear();

        protected void RaiseDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }
    }

    public interface AggregateRoot
    {
    }
}