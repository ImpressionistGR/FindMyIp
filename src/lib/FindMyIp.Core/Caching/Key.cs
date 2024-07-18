namespace FindMyIp.Caching;

/// <summary>
/// 
/// </summary>
public static class Key
{
    /// <summary>
    /// 
    /// </summary>
    private const string AddressesCount = "counter:{0}";

    /// <summary>
    /// 
    /// </summary>
    private const string CountryDetails = "details:{0}";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="countryCode"></param>
    /// <returns></returns>
    public static string GetAddressesCountKey(string countryCode)
    {
        return string.Format(AddressesCount, countryCode);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="countryCode"></param>
    /// <returns></returns>
    public static string GetCountryDetailsKey(string countryCode)
    {
        return string.Format(CountryDetails, countryCode);
    }
}
