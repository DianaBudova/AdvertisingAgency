using AdvertisingAgency.DAL;
using AdvertisingAgency.WebApi.Filters;
using Microsoft.EntityFrameworkCore;

// налаштовує веб-додаток і підключає контейнер залежностей через Autofac, що дозволяє гнучко керувати залежностями в усьому додатку.

var builder = WebApplication.CreateBuilder(args);

// Connection string
var connStr = builder.Configuration.GetConnectionString("Default")
             ?? throw new InvalidOperationException("Connection string 'Default' not found.");

// EF Core (also registered in DefaultModule for DI lifecycle)
builder.Services.AddDbContext<AdvertisingAgencyContext>(opt =>
    opt.UseSqlServer(connStr));

// MVC + global validation filter
builder.Services.AddControllers(o => o.Filters.Add<ValidationFilter>())
                .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AdvertisingAgencyContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();