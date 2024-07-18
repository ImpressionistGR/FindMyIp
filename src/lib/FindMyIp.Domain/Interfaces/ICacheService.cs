namespace FindMyIp.Domain.Interfaces;

using System;
using System.Threading.Tasks;

/// <summary>
/// 
/// </summary>
public interface ICacheService
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="key"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<IResult<T>> GetAsync<T>(string key);

    /// <summary>
    ///
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<IResult<long>> IncrementAsync(string key);

    /// <summary>
    ///
    /// </summary>
    /// <param name="key"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<IResult<object>> RemoveAsync(string key);

    /// <summary>
    ///
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="timeToLive"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<IResult<object>> SetAlwaysAsync<T>(string key, T value,
        TimeSpan timeToLive);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="ipLocation"></param>
    /// <param name="lastUpdated"></param>
    /// <returns></returns>
    Task<IResult<object>> SetAlwaysAsync(string ip, Dto.IpLocation ipLocation, DateTime lastUpdated);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="updatedInfo"></param>
    /// <param name="lastUpdated"></param>
    /// <param name="oldCountryCode"></param>
    /// <returns></returns>
    Task<IResult<object>> UpdateAlwaysAsync(string ip, Dto.IpLocation updatedInfo, DateTime lastUpdated,
        string oldCountryCode);
}
