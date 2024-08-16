using AutoMapper;
using Final_project.Areas.Admin.Model;
using Final_project.Entities;

namespace Final_project.Areas.Automapper
{
    public class SubCategoryProfile : Profile
    {
        public SubCategoryProfile() 
        {
            CreateMap<SubCategoryViewModel, SubCategory>()
                .ReverseMap();
        }
    }
}
