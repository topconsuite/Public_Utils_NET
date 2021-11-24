using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Telluria.Utils.Crud.Entities;
using Telluria.Utils.Crud.Lists;

namespace Telluria.Utils.Crud.Repositories
{
    public class BaseCrudRepository<TEntity> : IBaseCrudRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly DbContext _context;

        public BaseCrudRepository(DbContext context)
        {
            _context = context;
        }

        protected DbSet<TSpecificEntity> DbSet<TSpecificEntity>() where TSpecificEntity : BaseEntity
        {
            return _context.Set<TSpecificEntity>();
        }

        public virtual async Task AddAsync(params TEntity[] entities)
        {
            await AddAsync<TEntity>(entities);
        }

        public virtual async Task AddAsync<TSpecificEntity>(params TSpecificEntity[] entities)
            where TSpecificEntity : BaseEntity
        {
            foreach (var entity in entities)
                entity.CreatedAt = DateTime.Now.ToUniversalTime();

            DbSet<TSpecificEntity>().AddRange(entities);
        }

        public virtual async Task UpdateAsync(params TEntity[] entities)
        {
            await UpdateAsync<TEntity>(entities);
        }

        public virtual async Task UpdateAsync<TSpecificEntity>(params TSpecificEntity[] entities)
            where TSpecificEntity : BaseEntity
        {
            foreach (var entity in entities)
                entity.UpdatedAt = DateTime.Now.ToUniversalTime();

            DbSet<TSpecificEntity>().UpdateRange(entities);
        }

        /// <summary>
        /// "Soft Delete": set the property "Deleted" to TRUE.
        /// </summary>
        public virtual async Task SoftDeleteAsync(params TEntity[] entities)
        {
            await SoftDeleteAsync<TEntity>(entities);
        }

        /// <summary>
        /// "Soft Delete": set the property "Deleted" to TRUE.
        /// </summary>
        public virtual async Task SoftDeleteAsync<TSpecificEntity>(params TSpecificEntity[] entities)
            where TSpecificEntity : BaseEntity
        {
            foreach (var entity in entities)
            {
                entity.DeletedAt = DateTime.Now.ToUniversalTime();
                entity.Deleted = true;
            }

            DbSet<TSpecificEntity>().UpdateRange(entities);
        }

        /// <summary>
        /// Remove the registry permanently from the database.
        /// </summary>
        public virtual async Task RemoveAsync(params TEntity[] entities)
        {
            await RemoveAsync<TEntity>(entities);
        }

        /// <summary>
        /// Remove the registry permanently from the database.
        /// </summary>
        public virtual async Task RemoveAsync<TSpecificEntity>(params TSpecificEntity[] entities)
            where TSpecificEntity : BaseEntity
        {
            DbSet<TSpecificEntity>().RemoveRange(entities);
        }

        public virtual async Task<TEntity> GetAsync(Guid id, bool tracking = false, params string[] includeProperties)
        {
            return await GetAsync<TEntity>(id, tracking, includeProperties);
        }

        public virtual async Task<TSpecificEntity> GetAsync<TSpecificEntity>(Guid id, bool tracking = false, params string[] includeProperties)
            where TSpecificEntity : BaseEntity
        {
            var set = DbSet<TSpecificEntity>().AsQueryable();

            foreach (var includePropertie in includeProperties)
                set = set.Include(includePropertie);

            set = set.Where(t => t.Id == id);
            
            return await set.Tracking(tracking).FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Return first or default register (that matches the filter) not deleted (when using "Soft Delete").
        /// </summary>
        public virtual async Task<TEntity> FindAsync(bool tracking = false,
            Expression<Func<TEntity, bool>> filter = null,
            params string[] includeProperties)
        {
            return await FindAsync<TEntity>(tracking, filter, includeProperties);
        }

        /// <summary>
        ///     Return first or default register (that matches the filter) not deleted (when using "Soft Delete").
        /// </summary>
        public virtual async Task<TSpecificEntity> FindAsync<TSpecificEntity>(bool tracking = false,
            Expression<Func<TSpecificEntity, bool>> filter = null,
            params string[] includeProperties)
            where TSpecificEntity : BaseEntity
        {
            var set = DbSet<TSpecificEntity>().AsQueryable();

            foreach (var includePropertie in includeProperties)
                set = set.Include(includePropertie);

            if (filter != null)
                set = set.Where(filter);

            return await set.Tracking(tracking).FirstOrDefaultAsync();
        }

        /// <summary>
        ///     Return all registers (that matches the filter) not deleted (when using "Soft Delete").
        /// </summary>
        public virtual async Task<PagedList<TEntity>> ListAsync(uint page, uint perPage, bool tracking = false,
            Expression<Func<TEntity, bool>> filter = null,
            params string[] includeProperties)
        {
            return await ListAsync<TEntity>(page, perPage, tracking, filter, includeProperties);
        }

        /// <summary>
        ///     Return all registers (that matches the filter) not deleted (when using "Soft Delete").
        /// </summary>
        public virtual async Task<PagedList<TSpecificEntity>> ListAsync<TSpecificEntity>(uint page, uint perPage, bool tracking = false,
            Expression<Func<TSpecificEntity, bool>> filter = null,
            params string[] includeProperties)
            where TSpecificEntity : BaseEntity
        {
            var set = DbSet<TSpecificEntity>().AsQueryable();

            foreach (var includePropertie in includeProperties)
                set = set.Include(includePropertie);

            if (filter != null)
                set = set.Where(filter);

            return await set.PagedList(page, perPage, tracking);
        }

        /// <summary>
        ///     Return all registers (that matches the filter), including deleted ones (when using "Soft Delete").
        /// </summary>
        public virtual async Task<PagedList<TEntity>> ListAllAsync(uint page, uint perPage, bool tracking = false,
            Expression<Func<TEntity, bool>> filter = null,
            params string[] includeProperties)
        {
            return await ListAllAsync<TEntity>(page, perPage, tracking, filter, includeProperties);
        }

        /// <summary>
        ///     Return all registers (that matches the filter), including deleted ones (when using "Soft Delete").
        /// </summary>
        public virtual async Task<PagedList<TSpecificEntity>> ListAllAsync<TSpecificEntity>(uint page, uint perPage, bool tracking = false,
            Expression<Func<TSpecificEntity, bool>> filter = null,
            params string[] includeProperties)
            where TSpecificEntity : BaseEntity
        {
            var set = DbSet<TSpecificEntity>().AsQueryable();

            foreach (var includePropertie in includeProperties)
                set = set.Include(includePropertie);

            if (filter != null)
                set = set.Where(filter);

            return await set.IgnoreQueryFilters().PagedList(page, perPage, tracking);
        }

        public async Task Commit()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                await _context.DisposeAsync();
                throw e;
            }
        }
    }

    public static class TestExtensions
    {
        public static string ToSql<TEntity>(this IQueryable<TEntity> query) where TEntity : class
        {
            var enumerator = query.Provider.Execute<IEnumerable<TEntity>>(query.Expression).GetEnumerator();
            var relationalCommandCache = enumerator.Private("_relationalCommandCache");
            var selectExpression = relationalCommandCache.Private<SelectExpression>("_selectExpression");
            var factory = relationalCommandCache.Private<IQuerySqlGeneratorFactory>("_querySqlGeneratorFactory");

            var sqlGenerator = factory.Create();
            var command = sqlGenerator.GetCommand(selectExpression);

            string sql = command.CommandText;
            return sql;
        }

        private static object Private(this object obj, string privateField) => obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);
        private static T Private<T>(this object obj, string privateField) => (T)obj?.GetType().GetField(privateField, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj);
    }
}
