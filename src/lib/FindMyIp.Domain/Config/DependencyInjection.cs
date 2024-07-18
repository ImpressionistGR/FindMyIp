namespace FindMyIp.Domain.Config;

using Microsoft.Extensions.DependencyInjection;

using FindMyIp.Domain.Services;
using FindMyIp.Domain.Providers;
using FindMyIp.Domain.Interfaces;

/// <summary>
/// 
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<IIp2CProvider, Ip2CProvider>();
        services.AddScoped<IIpUpdateService, IpUpdateService>();
        services.AddScoped<IFindMyIpService, FindMyIpService>();
        return services;
    }
}
