using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ModelsRepository.IRepositries
{
    public interface IBaseRepository<T> where T : class
    {
        T GetById(int id);
        void SaveChanges();
        T RemoveObj(T obj);
        T CheckUserLogin(Expression<Func<T, bool>> user);
        T GetUserBy(Expression<Func<T, bool>> exp);
        T Get(Expression<Func<T, bool>> exp);
        IEnumerable<T> GetAll(Expression<Func<T, bool>> exp);
        T Add(T entity);
        int Count(Expression<Func<T, bool>> exp);
    }
}
