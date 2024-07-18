namespace FindMyIp.Domain.Providers;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using EventId = FindMyIp.Logging.EventId;

/// <summary>
/// 
/// </summary>
public class Ip2CProvider : IIp2CProvider
{

    /// <summary>
    /// 
    /// </summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="httpClient"></param>
    public Ip2CProvider(ILogger<Ip2CProvider> logger, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromSeconds(20);
        _httpClient.BaseAddress = new Uri("http://ip2c.org/");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ip"></param>
    /// <returns></returns>
    public async Task<IResult<Dto.IpLocation>> GetLocationAsync(string ip)
    {
        var vresult = default(IResult<Dto.IpLocation>);
        try {
            var response = await _httpClient.GetAsync(ip);

            if (!response.IsSuccessStatusCode) {
                return Result.Error<Dto.IpLocation>(HttpStatusCode.BadGateway,
                    null, EventId.Ip2CProviderGetLocationRequestFailed);
            }

            var content = await response.Content.ReadAsStringAsync();

            vresult = ValidateContent(content);

            if (vresult.IsError) {
                return Result.Error<Dto.IpLocation>(vresult);
            }

        } catch (Exception e) {
            return Result.Error<Dto.IpLocation>(HttpStatusCode.BadGateway,
                null, EventId.Ip2CProviderGetLocationRequestFailed);
        }

        return Result.Success(vresult.Data);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    private static IResult<Dto.IpLocation> ValidateContent(string content)
    {
        if (content == null) {
            return Result<Dto.IpLocation>.Error(HttpStatusCode.PreconditionFailed,
                null, EventId.Ip2CProviderGetLocationValidationError);
        }

        var arr = content.Split(';');

        if (arr.Length != 4 || arr[0] != "1") {
            return Result<Dto.IpLocation>.Error(HttpStatusCode.PreconditionFailed,
                null, EventId.Ip2CProviderGetLocationValidationError);
        }

        return Result.Success(new Dto.IpLocation {
            CountryName = arr[3],
            TwoLetterCode = arr[1],
            ThreeLetterCode = arr[2]
        });
    }
}
