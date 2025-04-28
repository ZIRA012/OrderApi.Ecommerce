
using System.Linq.Expressions;
using System.Net.Http.Json;
using FakeItEasy;
using FluentAssertions;
using OrderApi.Application.DTOs;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;
using OrderApi.Domain.Entities;

namespace Test.OrderApi.Services;

public class OrderServiceTest
{

    private readonly IOrderService orderServiceInterface;
    private readonly IOrder orderInterface;
    public OrderServiceTest()
    {
        orderInterface = A.Fake<IOrder>();
        orderServiceInterface = A.Fake<IOrderService>();
    }
    

    //Falso http manager
    public class FakeHttpMessageHandle(HttpResponseMessage response): HttpMessageHandler
    {
        private readonly HttpResponseMessage _response = response;
        
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        => Task.FromResult(_response);
    }
    
    // Creamos un cliente HTPP con el Manager falso 

    private static HttpClient CreateFakeHttpClient(object o)
    {
        var httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = JsonContent.Create(o)
        };
        var fakeHttpMessageHandle = new FakeHttpMessageHandle(httpResponseMessage);
        var _httpClient = new HttpClient(fakeHttpMessageHandle)
        {
            BaseAddress = new Uri("http://localhost")
        };
        return _httpClient;
    }


    [Fact]
    public async Task GetProduct_ValidProductID_ReturnProduct()
    {
        //Arrange
        int productId = 1;
        var productDTO = new ProductDTO(1, "Producto 1", 23, 15.0m);

        var _httpClient = CreateFakeHttpClient(productDTO);


        //Solo pasamos el http Cliente no la Iorder ni la resilencesPipelien
        var _orderService = new OrderSevice(null!, _httpClient, null!);

        //Actuamos
        var result = await _orderService.GetProduct(productId);

        //Assert 
        result.Should().NotBeNull();
        result.Id.Should().Be(productId);
        result.Name.Should().Be("Producto 1");
    }

    [Fact]
    public async Task GetProduct_NotFoun_ReturnError()
    { 
        //Arrange
        int productId = 1;
        var productDTO = new ProductDTO(1, "Producto 1", 23, 15.0m);

        var _httpClient = CreateFakeHttpClient(null!);


        //Solo pasamos el http Cliente no la Iorder ni la resilencesPipelien
        var _orderService = new OrderSevice(null!, _httpClient, null!);

        //Actuamos
        var result = await _orderService.GetProduct(productId);

        //Assert 
        result.Should().BeNull(); 
    }

    [Fact]
    public async Task GetOrdersByClientId_OrderExist_ReturnOrderDetails()
    {
        int clienId = 1;
        var orders = new List<Order>
        { new() {Id = 1, ProductId=1, ClientId = clienId, PurchaseQuantity=1, OrderedDate=DateTime.UtcNow}
        ,
          new() {Id = 1, ProductId=2, ClientId = clienId, PurchaseQuantity=1, OrderedDate=DateTime.UtcNow }
        };  
        
        //Mock Para GetOrdersBy Method

        A.CallTo(() => orderInterface.GetOrdersAsync
        (A<Expression<Func<Order, bool>>>.Ignored)).Returns(orders);
        var _orderService = new OrderSevice(orderInterface!, null!, null!);
        //Actuar

        var result = await _orderService.GetOrdersByClientIDAsync(clienId);

        result.Should().NotBeNull();
        result.Should().HaveCount(orders.Count);
        result.Should().HaveCountGreaterThanOrEqualTo(2);
    }


}
