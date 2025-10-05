
using System.Linq.Expressions;
using eCommerce.SharedLibrary.Responses;

namespace eCommerce.SharedLibrary.Interfaces;

public interface IGenericInterface<T> where T : class
{
    Task<Response> CreateAsync(T entity);
    Task<T> ReadByIdAsync(int id);
    Task<IEnumerable<T>> ReadAllAsync();
    Task<T> ReadByAsync(Expression<Func<T, bool>> predicate);
    Task<Response> UpdateAsync(T entity);
    Task<Response> DeleteAsync(T entity);
}
