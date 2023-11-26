using RecAll.Infrastructure.EventBus.Events;

namespace RecAll.Infrastructure.EventBus.Abstractions;

public interface IEventBus {
    void Publish(IntegrationEvent @event);
}