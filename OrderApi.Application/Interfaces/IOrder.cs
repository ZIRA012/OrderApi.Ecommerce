
using System.Linq.Expressions;
using ECommmerce.SharedLibrary.Interfaces;
using OrderApi.Domain.Entities;

namespace OrderApi.Application.Interfaces;

public interface IOrder:IGenericInterface<Order>
{

    //agregamos ottro metodo que funciona Expression
    //EF Cor es capaz traducir estas Expression en Peticiones SQL NNH
    Task<IEnumerable<Order>> GetOrdersAsync(Expression<Func<Order,bool>> predicate);
}
