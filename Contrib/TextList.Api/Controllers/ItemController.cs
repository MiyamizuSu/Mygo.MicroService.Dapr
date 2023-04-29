using Microsoft.AspNetCore.Mvc;
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
}