using OrderApi.Domain.Entities;
namespace OrderApi.Application.DTOs.Conversions;
public static class OrderConversion
{
    public static Order ToEntity(OrderDTO order) => new ()
    {
        Id = order.Id,
        ClientId = order.ClientId,
        ProductId = order.ProductId,
        OrderedDate = order.OrderedDate,
        PurchaseQuantity = order.PurchaseQuantity,
    };

    //CONVERTIMOS A DTOS sean varios o pocos


    public static (OrderDTO?, IEnumerable<OrderDTO>?) 
        FromEntity (Order? order, IEnumerable<Order>? orders )
    {
        //Si se mando solo 
        {
            if(order is not null || orders is null)
            {
                var singleOrder = new OrderDTO(
                    order!.Id,
                    order.ProductId,
                    order.ClientId,
                    order.PurchaseQuantity,
                    order.OrderedDate
                    );
                return (singleOrder, null);
            }
        }

        if(order is null || orders is not null)
        {
            var _orders = orders!.Select(o =>
            new OrderDTO(
                o.Id,
                o.ProductId,
                o.ClientId,
                o.PurchaseQuantity,
                o.OrderedDate
                ));
            return(null, _orders);
        }
        return(null, null);
    }
}
