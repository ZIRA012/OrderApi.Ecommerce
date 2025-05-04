
using System.Net.Http.Json;
using Azure;
using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using OrderApi.Domain.Entities;
using Polly;
using Polly.Registry;
using ECommmerce.SharedLibrary.Responses;
using System.Text.Json;

using Response = ECommmerce.SharedLibrary.Responses.Response;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace OrderApi.Application.Services;

public class OrderSevice(IOrder orderInterface,HttpClient httpClient,
    ResiliencePipelineProvider<string> resiliencePipeline, IHttpContextAccessor httpContextAccessor) : IOrderService
{
    //Get Product //Utilizamos la Product Api para que nos de el aproducto
    public async Task<ProductDTO> GetProduct(int ProductId)
    {
        //llamamos al cliente http ProductAPI
        //si Product no responde, la redeirigimos a la Gateway API
        var getProduct = await httpClient.GetAsync($"/api/Products/{ProductId}");
        if (!getProduct.IsSuccessStatusCode) //si no pudimos encontrar el producto
            return null!;


        var product = await getProduct.Content.ReadFromJsonAsync<ProductDTO>();

        return product!;
    }

    public async Task<Response> UpdateProductQuantity(ProductDTO productDTO, int purchasedQuantity)
    {
        var updateProductDTO = new ProductDTO
            (productDTO.Id,
            productDTO.Name,
            productDTO.Quantity - purchasedQuantity,
            productDTO.Price
            );
        
        //Actualizamos el producto
        var json = JsonSerializer.Serialize(updateProductDTO);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var token = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

        if (!string.IsNullOrEmpty(token))
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
        }


        var putResponse = await httpClient.PutAsync($"/api/Products", content);
        var message = await putResponse.Content.ReadAsStringAsync();
        if (!putResponse.IsSuccessStatusCode)
        {
            
            return new Response(false, $"Respuesta de la Api:{message}");
        }
        return new Response(true, $"Respuesta de la Api:{message}");
    }

    //GET USER // Utilizamos la Product Api para que nos de el usuaio
    public async Task<AppUserDTO> GetUser(int userId)
    {
        //llamamos al cliente http ProductAPI
        //si Product no responde, la redeirigimos a la Gateway API
        var getUser = await httpClient.GetAsync($"api/Authentication/{userId}");
        if (!getUser.IsSuccessStatusCode) //si no pudimos encontrar el producto
            return null!;


        var product = await getUser.Content.ReadFromJsonAsync<AppUserDTO>();
        return product!;
    }
    //GET ORDER DETAILS BY ID // Utilizamos las dos anteriores para buscar un orden y nos da todos los detalles
     public async Task<OrderDetailsDTO> GetOrderDetails(int orderId)
    {
       
        
        //Preparamos la orden

        var order =await orderInterface.FindByIdAsync(orderId);
        if (order is null || order!.Id <= 0)
            return null!;

        //si no conseguimos una respuesta intentara nuevamente
        var retryPipeline = resiliencePipeline.GetPipeline("my-retry-pipeline");

        //preparamos el producto

        var productDTO = await retryPipeline.ExecuteAsync(async token => await GetProduct(order.ProductId));

        //preparamos el cliente
        var appUserDTO = await retryPipeline.ExecuteAsync(async token => await GetUser(order.ClientId));
        
        //populate order details
        return new OrderDetailsDTO(
            order.Id,//1
            productDTO.Id,//2
            appUserDTO.Id,//3
            appUserDTO.Name,//4
            appUserDTO.Email,//5
            appUserDTO.Address,
            appUserDTO.TelephoneNumber,
            productDTO.Name,
            order.PurchaseQuantity,
            productDTO.Price,
            productDTO.Price * order.PurchaseQuantity,
            order.OrderedDate
            );
    }

    public async Task<IEnumerable<OrderDTO>> GetOrdersByClientIDAsync(int clientId)
    {

        var orders = await orderInterface.GetOrdersAsync(o => o.ClientId == clientId);
        if (!orders.Any()) return null!;


        //convertir a DTO
        var (_,_orders) = OrderConversion.FromEntity(null, orders);
        return _orders!;
    }

    public async Task<Response> PostOrder(Order order)
    {
        var retryPipeline = resiliencePipeline.GetPipeline("my-retry-pipeline");

        //BUSCAMOS EL producto del cual Queremos hacer Orden
        var productDTO = await retryPipeline.ExecuteAsync
            (async token => await GetProduct(order.ProductId));

        //Verificamos si tenemos cantidad suficiente de dicho producto
        if (order.PurchaseQuantity >= productDTO.Quantity)
            return new Response(false, "No tenemos suficiente candidad de ese producto");
        
        var UpdateResponse = await UpdateProductQuantity(productDTO, order.PurchaseQuantity);
        if(!UpdateResponse.Flag)
        {
            return new Response(false, "No se pudo Actualizar la cantidad de productos");
        }
        return new Response(true, "La Producto esta disponible y se actualizo el numero Disponible");

    }
}
