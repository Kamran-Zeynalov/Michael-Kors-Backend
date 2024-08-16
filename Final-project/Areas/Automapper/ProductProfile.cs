using AutoMapper;
using Final_project.Areas.Admin.Model;
using Final_project.Entities;

namespace Final_project.Areas.Automapper
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductViewModel>().ReverseMap();
        }

    }
}
