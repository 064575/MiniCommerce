using ApiGateway.Exceptions;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient("UserService", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["Services:UserService"]!);

    client.Timeout = TimeSpan.FromSeconds(5);

});
builder.Services.AddHttpClient("ProductService", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["Services:ProductService"]!);

    client.Timeout = TimeSpan.FromSeconds(5);
});


builder.Services.AddHttpClient("InventoryService", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["Services:InventoryService"]!);

    client.Timeout = TimeSpan.FromSeconds(5);

});


builder.Services.AddHttpClient("OrderService", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["Services:OrderService"]!);

    client.Timeout = TimeSpan.FromSeconds(5);
});



builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddCors(options =>
{
    options.AddPolicy("WebApp", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("WebApp");

app.UseExceptionHandler();

// Configure the HTTP request pipeline.

app.UseSwagger();
    app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
