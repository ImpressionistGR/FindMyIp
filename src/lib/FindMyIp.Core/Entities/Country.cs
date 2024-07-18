namespace FindMyIp.Entities;

using System;

/// <summary>
/// 
/// </summary>
public class Country
{
    #region Properties

    /// <summary>
    /// 
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string TwoLetterCode { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string ThreeLetterCode { get; set; }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    public Country()
    {
        CreatedAt = DateTime.UtcNow;
    }
}
