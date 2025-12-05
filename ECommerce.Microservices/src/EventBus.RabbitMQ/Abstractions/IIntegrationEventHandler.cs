using EventBus.RabbitMQ.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.RabbitMQ.Abstractions;

public interface IIntegrationEventHandler<in TIntegrationEvent>
    where TIntegrationEvent : IntegrationEvent
{
    Task Handle(TIntegrationEvent @event);
}
