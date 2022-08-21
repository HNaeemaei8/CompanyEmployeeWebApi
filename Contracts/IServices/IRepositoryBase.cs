using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.IServices
{
   public interface IRepositoryBase<T> where T:class
    {
        IQueryable<T> FindAll(bool trachchanges);
        IQueryable<T> FindByCaption(Expression<Func<T,bool>> expression, bool trachchanges);
        void Create(T Entity);
        void Update(T Entity);
        void Delete(T Entity);

    }
}
