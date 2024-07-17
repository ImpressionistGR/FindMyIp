﻿using Hangfire;

namespace FindMyIp.Infrastructure.Config;

using StackExchange.Redis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using FindMyIp.Data;
using FindMyIp.Caching;
using FindMyIp.Core.Config;
using FindMyIp.Domain.Providers;
using FindMyIp.Infrastructure.Data;
using FindMyIp.Infrastructure.Caching;

/// <summary>
/// 
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        ApplicationConfig configuration)
    {
        services.AddDbContext<AppDbContext>(opts => {
            opts.UseLazyLoadingProxies();
            opts.UseSqlServer(configuration.ConnectionStrings.Database);
        });
        
        services.AddScoped<IRepository, AppRepository>();
        
        services.AddHttpClient<IIp2CProvider, Ip2CProvider>();

        services.AddHangfire(conf => conf
            .UseSqlServerStorage(configuration.ConnectionStrings.HangfireDb));

        services.AddHangfireServer();

        var redisConfiguration = ConfigurationOptions.Parse(configuration.ConnectionStrings.Redis);
        redisConfiguration.AbortOnConnectFail = false;

        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(redisConfiguration));

        services.AddScoped<ICache>(x => {
            var database = x.GetRequiredService<IConnectionMultiplexer>()
                .GetDatabase();

            return new RedisCache(database);
        });
        
        return services;
    }
}