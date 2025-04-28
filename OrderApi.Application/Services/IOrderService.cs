using OrderApi.Application.DTOs;
using OrderApi.Domain.Entities;
using ECommmerce.SharedLibrary.Responses;
namespace OrderApi.Application.Services;

public interface IOrderService
{
    Task<IEnumerable<OrderDTO>> GetOrdersByClientIDAsync(int clientId);
    Task<OrderDetailsDTO> GetOrderDetails(int orderId);
    Task<AppUserDTO> GetUser(int userId);
    Task<ProductDTO> GetProduct(int ProductId);
    Task<Response> PostOrder(Order order);
}
