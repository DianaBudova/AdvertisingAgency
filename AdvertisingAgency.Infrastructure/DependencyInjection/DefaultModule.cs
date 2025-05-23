using Autofac;
using AdvertisingAgency.DAL;
using AdvertisingAgency.DAL.Abstractions;
using AdvertisingAgency.DAL.Repositories;
using AdvertisingAgency.BLL.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

// зберігає налаштування для реєстрації конкретних типів у контейнері залежностей, таких як контекст бази даних, репозиторії та сервіси.
namespace AdvertisingAgency.Infrastructure.DependencyInjection
{
    public class DefaultModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AdvertisingAgencyContext>()
                   .AsSelf()
                   .As<DbContext>()
                   .InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(Repository<>))
                   .As(typeof(IRepository<>))
                   .InstancePerLifetimeScope();

            builder.RegisterType<UnitOfWork>()
                   .As<IUnitOfWork>()
                   .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(typeof(ServiceService).Assembly)
                   .Where(t => t.Name.EndsWith("Service"))
                   .AsImplementedInterfaces()
                   .InstancePerLifetimeScope();
        }
    }
}
