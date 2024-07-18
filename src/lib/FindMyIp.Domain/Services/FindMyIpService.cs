namespace FindMyIp.Domain.Services;

using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

using FindMyIp.Data;
using FindMyIp.Caching;
using FindMyIp.Logging;
using FindMyIp.Domain.Interfaces;
using FindMyIp.Domain.Providers;

/// <summary>
/// 
/// </summary>
public partial class FindMyIpService : IFindMyIpService
{
    /// <summary>
    /// 
    /// </summary>
    private readonly IRepository _repo;

    /// <summary>
    /// 
    /// </summary>
    private readonly IIp2CProvider _ip2C;

    /// <summary>
    /// 
    /// </summary>
    private readonly ICacheService _cache;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cache"></param>
    /// <param name="repo"></param>
    /// <param name="ip2C"></param>
    public FindMyIpService(ICacheService cache, IRepository repo, IIp2CProvider ip2C)
    {
        _cache = cache;
        _repo = repo;
        _ip2C = ip2C;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ip"></param>
    /// <returns></returns>
    public async Task<IResult<Dto.IpLocation>> GetLocationAsync(string ip)
    {
        if (!IPAddress.TryParse(ip, out _)) {
            return Result.Error<Dto.IpLocation>(HttpStatusCode.BadRequest,
                $"Invalid IP address format: {ip}", EventId.FindMyIpServiceGetLocationValidationError);
        }

        var gresult = await _cache.GetAsync<Dto.IpLocation>(ip);

        if (gresult.IsSuccess) {
            return Result.Success(gresult.Data);
        }

        var ipAddress = await _repo.GetQueryable<Entities.IpAddress>()
            .Where(x => x.Ip == ip)
            .Include(x => x.Country)
            .FirstOrDefaultAsync();

        if (ipAddress != null) {
            return Result<Dto.IpLocation>.Success(
                new Dto.IpLocation {
                    CountryName = ipAddress.Country.Name,
                    TwoLetterCode = ipAddress.Country.TwoLetterCode,
                    ThreeLetterCode = ipAddress.Country.ThreeLetterCode
                });
        }

        var response = await _ip2C.GetLocationAsync(ip);

        if (response.IsError) {
            return Result.Error<Dto.IpLocation>(response);
        }

        var iresult = await InsertNewIpAsync(ip, response.Data);

        if (iresult.IsError) {
            return Result.Error<Dto.IpLocation>(iresult);
        }

        return Result.Success(response.Data);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="countryCodes"></param>
    /// <returns></returns>
    public async Task<IResult<List<Dto.CountryReport>>> GetReportAsync(string countryCodes)
    {
        var vresult = ValidateCountryCodes(countryCodes);

        if (vresult.IsError) {
            return Result.Error<List<Dto.CountryReport>>(vresult);
        }

        var countryCodesList = vresult.Data;
        var countryReportList = new List<Dto.CountryReport>();
        var missingCodes = new List<string>();

        foreach (var cc in countryCodesList)
        {
            var addressesCount = await _cache.GetAsync<int>(Key.GetAddressesCountKey(cc));

            if (addressesCount.IsError) {
                missingCodes.Add(cc);
                continue;
            }

            var countryDetails = await _cache.GetAsync<Dto.CountryDetails>(Key.GetCountryDetailsKey(cc));

            if (countryDetails.IsError) {
                missingCodes.Add(cc);
                continue;
            }

            countryReportList.Add(new Dto.CountryReport {
                AddressesCount = addressesCount.Data,
                CountryName = countryDetails.Data.CountryName,
                LastAddressUpdated = countryDetails.Data.LastAddressUpdated
            });
        }

        if (missingCodes.Count > 0) {
            var countryCodesParam = string.Join(",", missingCodes);
            var eresult = await _repo.ExecuteSqlRawToListAsync<Dto.CountryReport>(
                $"EXEC GetCountryReport @CountryCodes='{countryCodesParam}'");

            if (eresult.IsError) {
                return Result.Error<List<Dto.CountryReport>>(eresult);
            }

            if (eresult.Data != null) {
                countryReportList.AddRange(eresult.Data);
            }
        }

        return Result.Success(countryReportList);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="countryCodes"></param>
    /// <returns></returns>
    private static IResult<List<string>> ValidateCountryCodes(string countryCodes)
    {
        if (countryCodes == null) {
            return Result.Error<List<string>>(HttpStatusCode.PreconditionFailed,
                null, EventId.FindMyIpServiceGetReportValidationError);
        }

        var regex = MyRegex();

        if (!regex.IsMatch(countryCodes)) {
            return Result.Error<List<string>>(HttpStatusCode.PreconditionFailed,
                "Invalid country codes format", EventId.FindMyIpServiceGetReportValidationError);
        }

        var list = countryCodes.Split(',')
            .Select(code => code.Trim())
            .ToList();

        return Result.Success(list);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="ipLocation"></param>
    /// <returns></returns>
    private async Task<IResult<object>> InsertNewIpAsync(string ip, Dto.IpLocation ipLocation)
    {
        using var ts = Transactions.CreateTransactionScope();

        var country = await _repo.GetQueryable<Entities.Country>()
            .Where(e => e.TwoLetterCode == ipLocation.TwoLetterCode)
            .FirstOrDefaultAsync();

        var result = default(IResult<object>);
        if (country == null) {
            country = new Entities.Country {
                Name = ipLocation.CountryName,
                TwoLetterCode = ipLocation.TwoLetterCode,
                ThreeLetterCode = ipLocation.ThreeLetterCode
            };

            _repo.Add(country);

            result = await _repo.TryCommitAsync();
            if (result.IsError) {
                return Result.Error<Dto.IpLocation>(result);
            }
        }

        var ipAddress = new Entities.IpAddress {
            Ip = ip,
            CountryId = country.Id
        };

        _repo.Add(ipAddress);

        result = await _repo.TryCommitAsync();
        if (result.IsError) {
            return Result.Error<Dto.IpLocation>(result);
        }

        result = await _cache.SetAlwaysAsync(ip, ipLocation, ipAddress.UpdatedAt);
        if (result.IsError) {
            return Result.Error<object>(result);
        }

        ts.Complete();

        return Result.Success();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"^([A-Z]{2})(,[A-Z]{2})*$")]
    private static partial Regex MyRegex();
}
