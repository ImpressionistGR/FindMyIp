using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace FindMyIp.Domain.Services;

using System.Threading.Tasks;

using FindMyIp.Data;
using FindMyIp.Domain.Providers;
using FindMyIp.Domain.Interfaces;

/// <summary>
/// 
/// </summary>
public class IpUpdateService : IIpUpdateService
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
    /// <param name="ip2C"></param>
    /// <param name="repo"></param>
    public IpUpdateService(ICacheService cache, IIp2CProvider ip2C, IRepository repo)
    {
        _cache = cache;
        _ip2C = ip2C;
        _repo = repo;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task UpdateIpInformation()
    {
        const int batchSize = 100;
        var offset = 0;

        while (true)
        {
            var ipsToUpdate = await _repo
                .GetQueryable<Entities.IpAddress>()
                .OrderBy(i => i.Id)
                .Skip(offset)
                .Take(batchSize)
                .Include(i => i.Country)
                .ToListAsync();

            if (ipsToUpdate.Count == 0) {
                break;
            }

            foreach (var ip in ipsToUpdate) {

                var updatedInfo = await _ip2C.GetLocationAsync(ip.Ip);

                if (updatedInfo.IsError) {
                    continue;
                }

                using var ts = Transactions.CreateTransactionScope();

                var newCountry = await _repo.GetQueryable<Entities.Country>()
                    .Where(e => e.TwoLetterCode == updatedInfo.Data.TwoLetterCode)
                    .FirstOrDefaultAsync();

                var result = default(IResult<object>);
                if (newCountry == null) {
                    newCountry = new Entities.Country
                    {
                        Name = updatedInfo.Data.CountryName,
                        TwoLetterCode = updatedInfo.Data.TwoLetterCode,
                        ThreeLetterCode = updatedInfo.Data.ThreeLetterCode
                    };

                    _repo.Add(newCountry);

                    result = await _repo.TryCommitAsync();
                    if (result.IsError) {
                        continue;
                    }
                }

                ip.CountryId = newCountry.Id;
                ip.Update();

                _repo.Update(ip);

                result = await _repo.TryCommitAsync();

                if (result.IsError) {
                    continue;
                }

                result = await _cache.UpdateAlwaysAsync(ip.Ip, updatedInfo.Data, ip.UpdatedAt,
                    ip.Country.TwoLetterCode);

                if (result.IsError) {
                    continue;
                }

                ts.Complete();
            }

            offset += batchSize;
        }
    }
}
