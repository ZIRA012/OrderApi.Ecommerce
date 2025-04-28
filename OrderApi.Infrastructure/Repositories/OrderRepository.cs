using System.Linq.Expressions;
using ECommmerce.SharedLibrary.Logs;
using ECommmerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using OrderApi.Application.Interfaces;
using OrderApi.Domain.Entities;
using OrderApi.Infrastructure.Data;

namespace OrderApi.Infrastructure.Repositories;

public class OrderRepository(OrderDbContext context) : IOrder
{
    public  async Task<Response> CreateAsync(Order entity)
    {
        try
        {

            
          var order =context.Orders.Add(entity).Entity;
            await context.SaveChangesAsync();
            return order.Id > 0 ? new Response(true, "Orden Registrada Correctamente") :
                new Response(false, "Ocurrio un error regristrand la orden");
        }
        catch (Exception ex) 
        { 
        //Log de la excepcion Original

            LogException.LogExceptions(ex);
            //Mostrar el error

            return new Response(false, "Se produjo un error al realizar la orden");
        }
    }

    public async Task<Response> DeleteAsync(Order entity)
    {
        try
        {
            var orders = await FindByIdAsync(entity.Id);
            if (orders == null)
            {
                return new Response(false, "La orden a Eliminar no existe");

            }

            context.Orders.Remove(entity);
            await context.SaveChangesAsync();
            return new Response(true, "La orden se elimino");
        }
        catch (Exception ex)
        {
            //Log de la excepcion Original

            LogException.LogExceptions(ex);
            //Mostrar el error

            return new Response(false, "Se produjo un error al Eliminar la orden");
        }
    }

    public async Task<Order> FindByIdAsync(int id)
    {
        try
        {
            var order = await context.Orders!.FindAsync(id);
            return order is not null ? order : null!;
                
        }
        catch (Exception ex)
        {
            //Log de la excepcion Original

            LogException.LogExceptions(ex);
            //Mostrar el error

            throw new Exception( "Se produjo un al buscar la orden por ID");
        }
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        
        try
        {

            var orders = await context.Orders.AsNoTracking().ToListAsync();
            return orders is not null ? orders : null!;
        }
        catch (Exception ex)
        {
            //Log de la excepcion Original

            LogException.LogExceptions(ex);
            //Mostrar el error

            throw new Exception("Se produjo un al listar todas las ordenes");
        }
    }

    public async Task<Order> GetByAsync(Expression<Func<Order, bool>> predicate)
    {
        try
        {
            var order = await context.Orders.Where(predicate).FirstOrDefaultAsync()!;
            return order is not null ? order : null!;
        }
        catch (Exception ex)
        {
            //Log de la excepcion Original

            LogException.LogExceptions(ex);
            //Mostrar el error

            throw new Exception("Error al filtrar la orden"); 
        }
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync(Expression<Func<Order, bool>> predicate)
    {
        try
        {

           var orders = await context.Orders.Where(predicate).ToListAsync();
            return orders is not null ? orders : null!;
        }
        catch (Exception ex)
        {
            //Log de la excepcion Original

            LogException.LogExceptions(ex);
            //Mostrar el error

            throw new Exception("Error al filtrar las Ordenes");
        }
    }

    public async Task<Response> UpdateAsync(Order entity)
    {
        try
        {

           var order = await FindByIdAsync(entity.Id);
            if (order is null)
                return new Response(false, "Orden a modificar no encontrada");

            context.Entry(order).State = EntityState.Detached;
            context.Update(entity);
            await context.SaveChangesAsync();
            return  new Response(true,"Orden modificada correctamente");
        }
        catch (Exception ex)
        {
            //Log de la excepcion Original

            LogException.LogExceptions(ex);
            //Mostrar el error

            return new Response(false, "Se produjo un error al modificar la orden");
        }
    }
}
