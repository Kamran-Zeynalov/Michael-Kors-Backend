using AutoMapper;
using Final_project.Areas.Admin.Model;
using Final_project.Entities;

namespace Final_project.Areas.Automapper
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryViewModel>().ReverseMap();
        }
    }
}
