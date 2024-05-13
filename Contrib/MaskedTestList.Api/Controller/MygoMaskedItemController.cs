using Microsoft.AspNetCore.Mvc;
using RecAll.Contrib.MaskedTestList.Api.commands;

namespace RecAll.Contrib.MaskedTestList.Api.Controller;

[ApiController]
[Route("controller")]
public class MygoMaskedItemController
{
    [Route("create")]
    [HttpPost]
    public async Task<ActionResult<string>> createAsync([FromBody]createMaskedTextItemCommand command)
    {
        return "Mygo";
    }
}