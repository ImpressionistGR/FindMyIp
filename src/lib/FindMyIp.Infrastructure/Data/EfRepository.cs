namespace FindMyIp.Infrastructure.Data;

using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using FindMyIp.Data;
using FindMyIp.Logging;

/// <summary>
/// 
/// </summary>
public class EfRepository : IRepository
{
    /// <summary>
    /// 
    /// </summary>
    private readonly DbContext _dbContext;

    /// <summary>
    ///
    /// </summary>
    /// <param name="context"></param>
    public EfRepository(DbContext context)
    {
        _dbContext = context;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sql"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task<IResult<List<T>>> ExecuteSqlRawToListAsync<T>(string sql) where T : class
    {
        try {
            var list = await _dbContext.Set<T>().FromSqlRaw(sql).ToListAsync();
            return Result.Success(list);
        }
        catch (Exception e) {
            return Result.Error<List<T>>(HttpStatusCode.InternalServerError, e.Message);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="entity"></param>
    /// <typeparam name="T"></typeparam>
    public void Add<T>(T entity) where T : class
    {
        if (IsAttached(entity)) {
            return;
        }
        _dbContext.Set<T>().Add(entity);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="range"></param>
    /// <typeparam name="T"></typeparam>
    public void AddRange<T>(System.Collections.Generic.IEnumerable<T> range) where T : class
    {
        _dbContext.Set<T>().AddRange(range.Where(x => !IsAttached(x)));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="entity"></param>
    /// <typeparam name="T"></typeparam>
    public void Update<T>(T entity) where T : class
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="entity"></param>
    /// <typeparam name="T"></typeparam>
    public void Delete<T>(T entity) where T : class
    {
        _dbContext.Set<T>().Remove(entity);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="range"></param>
    /// <typeparam name="T"></typeparam>
    public void DeleteRange<T>(IEnumerable<T> range) where T : class
    {
        _dbContext.Set<T>().RemoveRange(range);
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public async Task<int> CommitAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<IResult<object>> TryCommitAsync()
    {
        try {
            await _dbContext.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception e) {
            return Result.Error(
                HttpStatusCode.InternalServerError, e.Message, EventId.RepositoryTryCommitFailed);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public int Commit()
    {
        return _dbContext.SaveChanges();
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IQueryable<T> GetQueryable<T>() where T : class
    {
        return _dbContext.Set<T>().AsQueryable();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetById<T>(params object[] id) where T : class
    {
        return _dbContext.Set<T>().Find(id);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public async Task<T> GetByIdAsync<T>(params object[] id) where T : class
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="entity"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private bool IsAttached<T>(T entity) where T : class
    {
        var entry = _dbContext.Entry(entity);
        return entry is not null && entry.State != EntityState.Detached;
    }
}
