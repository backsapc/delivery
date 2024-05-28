using CSharpFunctionalExtensions;

namespace Primitives
{
    public abstract class Aggregate<TKey> : Entity<TKey>, AggregateRoot where TKey : IComparable<TKey>
    {
        private readonly List<DomainEvent> _domainEvents = new();

        protected Aggregate(TKey id) : base(id)
        {
        }

        protected Aggregate()
        {
        }

        public IReadOnlyCollection<DomainEvent> GetDomainEvents() => _domainEvents.ToList();

        public void ClearDomainEvents() => _domainEvents.Clear();

        protected void RaiseDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }
    }

    public interface AggregateRoot
    {
        IReadOnlyCollection<DomainEvent> GetDomainEvents();
        void ClearDomainEvents();
    }
}