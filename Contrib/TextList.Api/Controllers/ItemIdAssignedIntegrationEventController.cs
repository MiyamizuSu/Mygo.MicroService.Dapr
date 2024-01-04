using Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecAll.Contrib.TextList.Api.IntegrationEvents;
using RecAll.Contrib.TextList.Api.Services;
using RecAll.Core.List.Domain.AggregateModels;
using RecAll.Infrastructure.EventBus;

namespace RecAll.Contrib.TextList.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemIdAssignedIntegrationEventController {
    private readonly TextListContext _textListContext;

    private readonly ILogger<ItemIdAssignedIntegrationEventController> _logger;

    public ItemIdAssignedIntegrationEventController(
        TextListContext textListContext,
        ILogger<ItemIdAssignedIntegrationEventController> logger) {
        _textListContext = textListContext;
        _logger = logger;
    }

    [Route("itemIdAssigned")]
    [HttpPost]
    [Topic(DaprEventBus.PubSubName, nameof(ItemIdAssignedIntegrationEvent))]
    public async Task HandleAsync(ItemIdAssignedIntegrationEvent @event) {
        if (@event.TypeId != ListType.Text.Id) {
            return;
        }

        _logger.LogInformation(
            "----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
            @event.Id, ProgramExtensions.AppName, @event);

        var textItem = await _textListContext.TextItems.FirstOrDefaultAsync(p =>
            p.Id == int.Parse(@event.ContribId));

        if (textItem is null) {
            _logger.LogWarning("Unknown TextItem id: {ItemId}", @event.ItemId);
            return;
        }

        textItem.ItemId = @event.ItemId;
        await _textListContext.SaveChangesAsync();

        _logger.LogInformation(
            "----- Integration event handled: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
            @event.Id, ProgramExtensions.AppName, @event);
    }
}