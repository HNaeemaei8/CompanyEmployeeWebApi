using Contracts.IServices;
using Entities.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T>  where T:class
    {
        protected CompanyEmployeeDbContext _dbContext;
        public RepositoryBase(CompanyEmployeeDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Create(T Entity)
        {
            _dbContext.Set<T>().Add(Entity);
        }

        public void Delete(T Entity)
        {
            _dbContext.Set<T>().Remove(Entity);
        }

        public IQueryable<T> FindAll(bool trachchanges)
        =>
            !trachchanges ?
             _dbContext.Set<T>()
             .AsNoTracking() :
             _dbContext.Set<T>();


        public IQueryable<T> FindByCaption(Expression<Func<T, bool>> expression, bool trachchanges)
        =>
            !trachchanges ?
            _dbContext.Set<T>()
            .Where(expression)
            .AsNoTracking() :
            _dbContext.Set<T>()
            .Where(expression);
            


        public void Update(T Entity)
        {
            _dbContext.Set<T>().Update(Entity);
        }
    }

}
