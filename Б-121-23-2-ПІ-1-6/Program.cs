using AdvertisingAgency.BLL.Mapping;
using AdvertisingAgency.DAL;
using AdvertisingAgency.WebApi.Filters;
using AdvertisingAgency.Infrastructure.DependencyInjection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Б_121_23_2_ПІ_1_6.Mappings;
using AdvertisingAgency.BLL.Mappings;

// налаштовує веб-додаток і підключає контейнер залежностей через Autofac, що дозволяє гнучко керувати залежностями в усьому додатку.

var builder = WebApplication.CreateBuilder(args);

// Replace default service provider with Autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(cb => cb.RegisterModule<DefaultModule>());

// Connection string
var connStr = builder.Configuration.GetConnectionString("Default")
             ?? throw new InvalidOperationException("Connection string 'Default' not found.");

// EF Core (also registered in DefaultModule for DI lifecycle)
builder.Services.AddDbContext<AdvertisingAgencyContext>(opt =>
    opt.UseSqlServer(connStr));

// MVC + global validation filter
builder.Services.AddControllers(o => o.Filters.Add<ValidationFilter>())
                .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase);

// AutoMapper (use BLL profiles)
builder.Services.AddAutoMapper(typeof(CategoryMappingProfileBL).Assembly);
builder.Services.AddAutoMapper(typeof(DiscountMappingProfileBL).Assembly);
builder.Services.AddAutoMapper(typeof(ServiceMappingProfileBL).Assembly);
builder.Services.AddAutoMapper(typeof(QuickOrderMappingProfileBL).Assembly);
builder.Services.AddAutoMapper(typeof(OrderMappingProfileBL).Assembly);
builder.Services.AddAutoMapper(typeof(UserMappingProfileBL).Assembly);
builder.Services.AddAutoMapper(typeof(PrintProductMappingProfileBL).Assembly);

// AutoMapper (use PL profiles)
builder.Services.AddAutoMapper(typeof(AuthMappingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(CategoryMappingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(DiscountMappingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(ServiceMappingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(QuickOrderMappingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(OrderMappingProfile).Assembly);
builder.Services.AddAutoMapper(typeof(PrintProductMappingProfile).Assembly);

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