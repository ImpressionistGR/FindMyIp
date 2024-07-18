namespace FindMyIp.Domain.Interfaces;

using System.Threading.Tasks;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public interface IFindMyIpService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ip"></param>
    /// <returns></returns>
    Task<IResult<Dto.IpLocation>> GetLocationAsync(string ip);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="countryCodes"></param>
    /// <returns></returns>
    Task<IResult<List<Dto.CountryReport>>> GetReportAsync(string countryCodes);
}