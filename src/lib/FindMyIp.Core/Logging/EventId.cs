namespace FindMyIp.Logging;

/// <summary>
/// 
/// </summary>
public enum EventId
{
    /// <summary>
    ///
    /// </summary>
    Undefined = 0,

    /// <summary>
    ///
    /// </summary>
    GenericError = 1,

    /// <summary>
    ///
    /// </summary>
    InternalServerError = 500,

    /// <summary>
    /// 
    /// </summary>
    RepositoryTryCommitFailed = 600,

    /* FindMyIpService */

    /// <summary>
    /// 
    /// </summary>
    FindMyIpServiceGetLocationValidationError = 1000,

    /// <summary>
    /// 
    /// </summary>
    Ip2CProviderGetLocationValidationError = 1001,

    /// <summary>
    /// /
    /// </summary>
    Ip2CProviderGetLocationRequestFailed = 1002,

    /// <summary>
    ///
    /// </summary>
    FindMyIpServiceGetReportValidationError = 1003
}
