namespace FindMyIp.Entities;

using System;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// 
/// </summary>
public class IpAddress
{
    #region Properties

    /// <summary>
    /// 
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string Ip { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int CountryId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    #endregion

    #region Navigation

    public virtual Country Country { get; set; }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    public IpAddress()
    {
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Update()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}