using DataAccessRepository.Context;
using ModelsRepository.IRepositries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessRepository.Repositries
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected MainContext _context;

        public BaseRepository(MainContext context)
        {
            _context = context;
        }

        public T Add(T entity)
        {
            _context.Set<T>().Add(entity);
            _context.SaveChanges();

            return entity;
        }
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
        public T CheckUserLogin(Expression<Func<T, bool>> user)
        {
            IQueryable<T> query = _context.Set<T>();

            return query.SingleOrDefault(user);
        }
        public T Get(Expression<Func<T, bool>> exp)
        {
            IQueryable<T> query = _context.Set<T>();

            return query.FirstOrDefault(exp);
        }
        public IEnumerable<T> GetAll(Expression<Func<T, bool>> exp)
        {
            IQueryable<T> query = _context.Set<T>();

            return query.Where(exp).ToArray();
        }
        public T GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public int Count(Expression<Func<T, bool>> exp)
        {
            IQueryable<T> query = _context.Set<T>();

            return query.Count(exp);
        }

        public T GetUserBy(Expression<Func<T, bool>> exp)
        {
            return _context.Set<T>().FirstOrDefault(exp);
        }

        public T RemoveObj(T obj)
        {
            _context.Set<T>().Remove(obj);
            _context.SaveChanges();
            return obj;
        }
    }
}
