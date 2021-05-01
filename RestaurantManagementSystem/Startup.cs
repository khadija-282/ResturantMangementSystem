using AutoMapper;
using Microsoft.Owin;
using Owin;
using RestaurantManagementSystem.Models;
using RestaurantManagementSystem.ViewModels;

[assembly: OwinStartupAttribute(typeof(RestaurantManagementSystem.Startup))]
namespace RestaurantManagementSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Item, ItemViewModel>().ReverseMap();
                cfg.CreateMap<Order, OrderViewModel>().ReverseMap();
                cfg.CreateMap<OrderDetail, OrderDetailViewModel>().ReverseMap();
            });
            ConfigureAuth(app);
            Common.CSNCommon.LoadConfigurations();
        }
    }
}
