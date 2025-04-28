
using System.Linq.Expressions;
using ECommmerce.SharedLibrary.Responses;
namespace ECommmerce.SharedLibrary.Interfaces
{
    public interface IGenericInterface<T> where T : class //por el momento solo hereda IProduct
    {

        //Recibe una entidad T(Clase) y devuelve un respones
        //Task hace que se ejecute en un hilo ya que se hace de manera asincrona

        Task<Response> CreateAsync(T entity); //Create

        Task<Response> UpdateAsync(T entity);

        Task<Response> DeleteAsync(T entity); 

        Task<IEnumerable<T>> GetAllAsync(); //READ

        Task<T> FindByIdAsync (int id);
        Task<T> GetByAsync(Expression<Func<T, bool>> predicate);
    }
}
