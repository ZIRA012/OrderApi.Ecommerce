using OrderApi.Infrastructure.DependencyInjection;
using OrderApi.Application.DependecyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructureService(builder.Configuration);
builder.Services.AddAplicationservice(builder.Configuration);

var app = builder.Build();

app.UserInfrastructurePolicy();
// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();



//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
