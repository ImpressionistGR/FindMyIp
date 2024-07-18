namespace FindMyIp.Domain.Services;

using System;
using System.Net;
using StackExchange.Redis;
using System.Threading.Tasks;

using FindMyIp.Caching;
using FindMyIp.Extensions;
using FindMyIp.Domain.Interfaces;

/// <summary>
///
/// </summary>
public class CacheService : ICacheService
{
    /// <summary>
    ///
    /// </summary>
    private readonly ICache _cache;

    /// <summary>
    ///
    /// </summary>
    /// <param name="cache"></param>
    public CacheService(ICache cache)
    {
        _cache = cache;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="ipLocation"></param>
    /// <param name="lastUpdated"></param>
    /// <returns></returns>
    public async Task<IResult<object>> SetAlwaysAsync(string ip, Dto.IpLocation ipLocation, DateTime lastUpdated)
    {
        try {
            var transaction = _cache.CreateTransaction();
            _ = transaction.StringSetAsync(ip, ipLocation.ToJson(), TimeSpan.MaxValue, When.Always);
            _ = transaction.StringIncrementAsync(Key.GetAddressesCountKey(ipLocation.TwoLetterCode));
            var countryDetails = new Dto.CountryDetails {
                CountryName = ipLocation.CountryName,
                LastAddressUpdated = lastUpdated
            };
            _ = transaction.StringSetAsync(Key.GetCountryDetailsKey(ipLocation.TwoLetterCode),
                countryDetails.ToJson(), TimeSpan.MaxValue, When.Always);
            var committed = await transaction.ExecuteAsync();
            return committed
                ? Result.Success()
                : Result.Error<object>(HttpStatusCode.InternalServerError, "transaction not commited");
        }
        catch (Exception e) {
            return Result.Error<object>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="updatedInfo"></param>
    /// <param name="lastUpdated"></param>
    /// <param name="oldCountryCode"></param>
    /// <returns></returns>
    public async Task<IResult<object>> UpdateAlwaysAsync(string ip, Dto.IpLocation updatedInfo, 
        DateTime lastUpdated, string oldCountryCode)
    {
        try {
            var transaction = _cache.CreateTransaction();

            var countryDetails = new Dto.CountryDetails {
                CountryName = updatedInfo.CountryName,
                LastAddressUpdated = lastUpdated
            };

            _ = transaction.StringSetAsync(Key.GetCountryDetailsKey(updatedInfo.TwoLetterCode),
                countryDetails.ToJson(), TimeSpan.MaxValue, When.Always);

            if (oldCountryCode != updatedInfo.TwoLetterCode) {
                _ = transaction.StringSetAsync(ip, updatedInfo.ToJson(), TimeSpan.MaxValue, When.Always);
                _ = transaction.StringIncrementAsync(Key.GetAddressesCountKey(updatedInfo.TwoLetterCode));
                _ = transaction.StringIncrementAsync(Key.GetAddressesCountKey(oldCountryCode), -1);
            }

            var committed = await transaction.ExecuteAsync();

            return committed
                ? Result.Success()
                : Result.Error<object>(HttpStatusCode.InternalServerError, "transaction not commited");
        } catch (Exception e) {
            return Result.Error<object>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<IResult<long>> IncrementAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) {
            Result.Error<long>(HttpStatusCode.BadRequest, $"null or empty {nameof(key)}");
        }

        try {
            var res = await _cache.IncrementAsync(key);
            return Result.Success(res);
        }
        catch (Exception e) {
            return Result.Error<long>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="key"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task<IResult<T>> GetAsync<T>(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) {
            Result.Error<T>(HttpStatusCode.BadRequest, $"null or empty {nameof(key)}");
        }

        try {
            var res = await _cache.GetAsync<T>(key);

            if (res == null) {
                return Result.Error<T>(HttpStatusCode.NotFound, "key does not exist");
            }

            return Result.Success(res);
        }
        catch (Exception e) {
            return Result.Error<T>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<IResult<object>> RemoveAsync(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) {
            Result<object>.Error(HttpStatusCode.BadRequest, $"null or empty {nameof(key)}");
        }

        return await _cache.RemoveAsync(key)
            ? Result.Success()
            : Result.Error(HttpStatusCode.NotFound, "");
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="timeToLive"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task<IResult<object>> SetAlwaysAsync<T>(string key, T value, TimeSpan timeToLive)
    {
        if (string.IsNullOrWhiteSpace(key)) {
            return Result.Error(HttpStatusCode.BadRequest, $"null or empty {nameof(key)}");
        }

        if (value is null) {
            return Result.Error(HttpStatusCode.BadRequest, $"null {nameof(value)}");
        }

        try {
            var res = await _cache.SetAlwaysAsync(key, value, timeToLive);
            return Result.Success();
        }
        catch (Exception e) {
            return Result.Error(HttpStatusCode.InternalServerError, e.Message);
        }
    }
}
