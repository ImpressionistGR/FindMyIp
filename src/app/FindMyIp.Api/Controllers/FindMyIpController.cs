namespace FindMyIp.Api.Controllers;

using Microsoft.AspNetCore.Mvc;

using FindMyIp.Domain.Interfaces;

[Route("[controller]")]
public class FindMyIpController : Controller
{
    private readonly IFindMyIpService _findMyIpService;

    /// <summary>
    ///
    /// </summary>
    /// <param name="findMyIpService"></param>
    public FindMyIpController(IFindMyIpService findMyIpService)
    {
        _findMyIpService = findMyIpService;
    }

    [HttpGet("{ip}")]
    public async Task<IActionResult> GetLocationAsync(string ip)
    {
        var result = await _findMyIpService.GetLocationAsync(ip);

        if (result.IsError) {
            return StatusCode((int)result.ErrorCode, result.ErrorMessage);
        }

        return Ok(result.Data);
    }

    [HttpGet("report/{countryCodes}")]
    public async Task<IActionResult> GetReportAsync(string countryCodes)
    {
        var result = await _findMyIpService.GetReportAsync(countryCodes);

        if (result.IsError) {
            return StatusCode((int)result.ErrorCode, result.ErrorMessage);
        }

        return Ok(result.Data);
    }
}
