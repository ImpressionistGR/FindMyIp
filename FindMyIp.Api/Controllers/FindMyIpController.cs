using FindMyIp.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FindMyIp.Api.Controllers;

[Route("[controller]")]
public class FindMyIpController : Controller
{
    private readonly IFindMyIpService _findMyIpService;

    public FindMyIpController(IFindMyIpService findMyIpService)
    {
        _findMyIpService = findMyIpService;
    }

    [HttpGet("{ip}")]
    public async Task<IActionResult> GetLocationAsync(string ip)
    {
        var result = await _findMyIpService.GetLocationAsync(ip);

        return result.IsSuccess
            ? Ok(result.Data)
            : StatusCode((int)result.ErrorCode, result.ErrorMessage);
    }

    [HttpGet("report/{countryCodes}")]
    public async Task<IActionResult> GetReportAsync(string countryCodes)
    {
        var result = await _findMyIpService.GetReportAsync(countryCodes);

        return result.IsSuccess
            ? Ok(result.Data)
            : StatusCode((int)result.ErrorCode, result.ErrorMessage);
    }
}
