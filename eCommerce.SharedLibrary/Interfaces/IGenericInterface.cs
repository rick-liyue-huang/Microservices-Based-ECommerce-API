
using eCommerce.SharedLibrary.Responses;

namespace eCommerce.SharedLibrary.Interfaces;

public interface IGenericInterface<T> where T : class
{
    Task<Response> CreateAsync(T entity);
}
