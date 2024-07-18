using FindMyIp.Domain.Dto;

namespace FindMyIp.Domain.Providers;

using System.Threading.Tasks;

/// <summary>
/// 
/// </summary>
public interface IIp2CProvider
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ip"></param>
    /// <returns></returns>
    Task<IResult<IpLocation>> GetLocationAsync(string ip);
}