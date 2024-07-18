namespace FindMyIp.Config;

using Microsoft.Extensions.Configuration;

/// <summary>
/// 
/// </summary>
public class ConnectionConfig
{
    /// <summary>
    ///
    /// </summary>
    [ConfigurationKeyName("redis")]
    public string Redis { get; set; }

    /// <summary>
    ///
    /// </summary>
    [ConfigurationKeyName("findmyip_db")]
    public string Database { get; set; }

    /// <summary>
    ///
    /// </summary>
    [ConfigurationKeyName("hangfire_db")]
    public string HangfireDb { get; set; }
}
