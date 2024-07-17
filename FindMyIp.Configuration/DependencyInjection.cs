namespace FindMyIp.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using FindMyIp.Core.Config;
using FindMyIp.Domain.Config;
using FindMyIp.Infrastructure.Config;

/// <summary>
/// 
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static IServiceCollection AddFindMyIp(this IServiceCollection services, IConfiguration config)
    {
        var appConfig = config.Get<ApplicationConfig>();
        return services
            .AddSingleton(appConfig)
            .AddDomain()
            .AddInfrastructure(appConfig);
    }
}