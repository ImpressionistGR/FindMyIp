namespace FindMyIp.Infrastructure.Data;

public sealed class AppRepository : EfRepository
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="context"></param>
    public AppRepository(AppDbContext context) : base(context)
    { }
}