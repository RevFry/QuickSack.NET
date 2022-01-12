using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuickSack.Server.Code;

namespace QuickSack.Net.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NextController : ControllerBase
{
    private readonly IFeedFactory feedFactory;
    public NextController(IFeedFactory factory) => feedFactory = factory;

    [HttpGet]
    public async Task<IActionResult> GetNext()
    {
        var res = await feedFactory.NextEpisode();
        return Ok(res);
    }
}
