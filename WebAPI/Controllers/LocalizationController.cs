using Main.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace WebAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class LocalizationController : ControllerBase
{
    private readonly IStringLocalizer _localizer;

    public LocalizationController(IStringLocalizer localizer)
    {
        _localizer = localizer;
    }

    [HttpGet]
    public IActionResult GetLocalization()
    {
        var localizations = _localizer.GetAllStrings().Select(x => new LocalizationDto
        {
            Key = x.Name,
            Value = x.Value
        });

        return Ok(localizations);
    }
}
