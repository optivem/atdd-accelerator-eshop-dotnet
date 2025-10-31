using Optivem.AtddAccelerator.EShop.Monolith.Api.Exceptions;
using Optivem.AtddAccelerator.EShop.Monolith.Core.Repositories;
using Optivem.AtddAccelerator.EShop.Monolith.Core.Services;
using Optivem.AtddAccelerator.EShop.Monolith.Core.Services.External;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Register application services
builder.Services.AddSingleton<OrderRepository>();
builder.Services.AddSingleton<OrderService>();
builder.Services.AddHttpClient<ErpGateway>();

// Add exception handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();