using ECommmerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;

namespace OrderApi.Presentation.Controllers;
[Route("api/[Controller]")]
[ApiController]
//[Authorize]
public class OrdersController(IOrder orderInterface, IOrderService orderService) : ControllerBase
{
    [HttpGet]//READ
    public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders()
    {
        var orders = await orderInterface.GetAllAsync();
        if (orders is null)
        {
            return NotFound("No existen ordenes en la base de datos");
        }

        var (_, list) = OrderConversion.FromEntity(null, orders);
        
        return !list!.Any() ? NotFound("No se encontro") : Ok(list);

    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDTO>> GetOrder(int id)
    {
        var order = await orderInterface.FindByIdAsync(id);
        if (order == null)
            return NotFound($"Orden con ID:{id} no fue encontrado");
        //Respondemos con el formato DTO, Siempre respondemos conDTO
        var (_order, _) = OrderConversion.FromEntity(order, null);
        return Ok(_order);


    }

    [HttpGet("client/{ClientId:int}")]
    public async Task<ActionResult<OrderDTO>> GetClientOrders(int ClientId)
    {
        if (ClientId <= 0)
            return BadRequest("Datos invalidos");

        var orders = await orderService.GetOrdersByClientIDAsync(ClientId);
        return !orders.Any() ? NotFound(null) : Ok(orders);
    }

    [HttpGet("details/{orderId:int}")]
    public async Task<ActionResult<OrderDetailsDTO>> GerOrderDetails(int orderId)
    {
        if (orderId <= 0)
            return BadRequest("Datos invalidos");

        var orderDetails = await orderService.GetOrderDetails(orderId);
        return orderDetails.OrdeID > 0 ? Ok(orderDetails) : NotFound("La orden no existe");
    }





    [HttpPost]
    public async Task<ActionResult<Response>> CreateOrder(OrderDTO orderDTO)
    {
        if (!ModelState.IsValid)
            return BadRequest("Los datos proporcionados no son validos");

        //convertimos a entidada para luego mandarla a la DB
        var getEntity = OrderConversion.ToEntity(orderDTO);
        var verify = await orderService.PostOrder(getEntity);
        if(!verify.Flag)//El Producto NO tiene la cantidad necesaria-> No se puede hacer la orden
            return BadRequest(verify.Message);
        var response = await orderInterface.CreateAsync(getEntity);
        return response.Flag ? Ok(response) : BadRequest("No se registro la Orden");

    }

    [HttpPut]
    public async Task<ActionResult<Response>> UpdateOrder(OrderDTO orderDTO)
    {
        if (!ModelState.IsValid)
            return BadRequest("Los datos de la Orden para Actualizar no son validos");

        //Convetimos  a entidad para luego actualizarla en la DB

        var getEntity = OrderConversion.ToEntity(orderDTO);
        var response = await orderInterface.UpdateAsync(getEntity);
        return response.Flag ? Ok(response) : BadRequest("No se Acutalizo la Orden");
    }

    // pide el id para eliminar
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<Response>> DeleteOrder(int id)
    {
        var order = await orderInterface.FindByIdAsync(id);
        if (order is null)
            return BadRequest($"La orden con id {id}no se encontro");
        var response = await orderInterface.DeleteAsync(order);
        return response.Flag ? Ok(response) : BadRequest("La orden se encontro pero no se puedo eliminar");
    }
}