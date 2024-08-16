using AutoMapper;
using Final_project.Areas.Admin.Model;
using Final_project.Entities;

namespace Final_project.Areas.Automapper
{
    public class SubSubCategoryProfile : Profile
    {
        public SubSubCategoryProfile()
        {
            CreateMap<SubSubCategoryViewModel, SubSubCategory>()
                .ReverseMap();
        }
    }
}
