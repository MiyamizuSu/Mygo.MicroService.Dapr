using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecAll.Contrib.TextList.Api.Commands;
using RecAll.Contrib.TextList.Api.Models;
using RecAll.Contrib.TextList.Api.Services;

namespace RecAll.Contrib.TextList.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemController {
    private readonly TextListContext _textListContext;
    private readonly IIdentityService _identityService;

    public ItemController(TextListContext textListContext,
        IIdentityService identityService) {
        _textListContext = textListContext ??
            throw new ArgumentNullException(nameof(textListContext));
        _identityService = identityService ??
            throw new ArgumentNullException(nameof(identityService));
    }

    [Route("create")]
    [HttpPost]
    // ServiceResultViewModel<string>
    public async Task<ActionResult<string>> CreateAsync(
        [FromBody] CreateTextItemCommand command) {
        var textItem = new TextItem {
            Content = command.Content,
            UserIdentityGuid = _identityService.GetUserIdentityGuid(),
            IsDeleted = false
        };
        var textItemEntity = _textListContext.Add(textItem);
        await _textListContext.SaveChangesAsync();

        // return ServiceResult<string>
        //     .CreateSucceededResult(textItemEntity.Entity.Id.ToString())
        //     .ToServiceResultViewModel();
        return textItemEntity.Entity.Id.ToString();
    }

    [Route("update")]
    [HttpPost]
    public async Task<ActionResult> UpdateAsync(
        [FromBody] UpdateTextItemCommand command) {
        var userIdentityGuid = _identityService.GetUserIdentityGuid();

        var textItem = await _textListContext.TextItems.FirstOrDefaultAsync(p =>
            p.Id == command.Id && p.UserIdentityGuid == userIdentityGuid &&
            !p.IsDeleted);

        if (textItem is null) {
            // return ServiceResult
            //     .CreateFailedResult($"Unknown TextItem id: {command.Id}")
            //     .ToServiceResultViewModel();
            return new BadRequestResult();
        }

        textItem.Content = command.Content;
        await _textListContext.SaveChangesAsync();

        // return ServiceResult.CreateSucceededResult().ToServiceResultViewModel();
        return new OkResult();
    }

    [Route("get/{id}")]
    [HttpGet]
    // ServiceResultViewModel<TextItemViewModel>
    public async Task<ActionResult<TextItem>> GetAsync(int id) {
        var userIdentityGuid = _identityService.GetUserIdentityGuid();

        var textItem = await _textListContext.TextItems.FirstOrDefaultAsync(p =>
            p.Id == id && p.UserIdentityGuid == userIdentityGuid &&
            !p.IsDeleted);

        if (textItem is null) {
            // return ServiceResult<TextItemViewModel>
            //     .CreateFailedResult($"Unknown TextItem id: {id}")
            //     .ToServiceResultViewModel();
            return new BadRequestResult();
        }

        return textItem is null
            // ? ServiceResult<TextItemViewModel>
            //     .CreateFailedResult($"Unknown TextItem id: {id}")
            //     .ToServiceResultViewModel()
            ? new BadRequestResult()
            // : ServiceResult<TextItemViewModel>
            //     .CreateSucceededResult(new TextItemViewModel {
            //         Id = textItem.Id,
            //         ItemId = textItem.ItemId,
            //         Content = textItem.Content
            //     }).ToServiceResultViewModel();
            : textItem;
    }

    [Route("getByItemId/{itemId}")]
    [HttpGet]
    // ServiceResultViewModel<TextItemViewModel>
    public async Task<ActionResult<TextItem>> GetByItemId(int itemId) {
        var userIdentityGuid = _identityService.GetUserIdentityGuid();

        var textItem = await _textListContext.TextItems.FirstOrDefaultAsync(p =>
            p.ItemId == itemId && p.UserIdentityGuid == userIdentityGuid &&
            !p.IsDeleted);

        if (textItem is null) {
            // return ServiceResult<TextItemViewModel>
            //     .CreateFailedResult($"Unknown TextItem with ItemID: {itemId}")
            //     .ToServiceResultViewModel();
            return new BadRequestResult();
        }

        return textItem is null
            // ? ServiceResult<TextItemViewModel>
            //     .CreateFailedResult($"Unknown TextItem with ItemID: {itemId}")
            //     .ToServiceResultViewModel()
            ? new BadRequestResult()
            // : ServiceResult<TextItemViewModel>
            //     .CreateSucceededResult(new TextItemViewModel {
            //         Id = textItem.Id,
            //         ItemId = textItem.ItemId,
            //         Content = textItem.Content
            //     }).ToServiceResultViewModel();
            : textItem;
    }

    [Route("getItems")]
    [HttpPost]
    // ServiceResultViewModel<IEnumerable<TextItemViewModel>>
    public async Task<ActionResult<IEnumerable<TextItem>>> GetItemsAsync(
        GetItemsCommand command) {
        var itemIds = command.Ids.ToList();
        var userIdentityGuid = _identityService.GetUserIdentityGuid();

        var textItems = await _textListContext.TextItems.Where(p =>
                p.ItemId.HasValue && itemIds.Contains(p.ItemId.Value) &&
                p.UserIdentityGuid == userIdentityGuid && !p.IsDeleted)
            .ToListAsync();

        if (textItems.Count != itemIds.Count) {
            var missingIds = string.Join(",",
                itemIds.Except(textItems.Select(p => p.ItemId.Value))
                    .Select(p => p.ToString()));

            // return ServiceResult<IEnumerable<TextItemViewModel>>
            //     .CreateFailedResult($"Unknown Item id: {missingIds}")
            //     .ToServiceResultViewModel();
            return new BadRequestResult();
        }

        textItems.Sort((x, y) =>
            itemIds.IndexOf(x.ItemId.Value) - itemIds.IndexOf(y.ItemId.Value));

        // return ServiceResult<IEnumerable<TextItemViewModel>>
        //     .CreateSucceededResult(textItems.Select(p => new TextItemViewModel {
        //         Id = p.Id, ItemId = p.ItemId, Content = p.Content
        //     })).ToServiceResultViewModel();
        return textItems;
    }
}