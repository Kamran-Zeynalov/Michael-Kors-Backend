using AutoMapper;
using Final_project.Areas.Admin.Model;
using Final_project.Entities;

namespace Final_project.Areas.Automapper
{
    public class SizeProfile : Profile
    {
        public SizeProfile()
        {
            CreateMap<Size, SizeViewModel>().ReverseMap();
        }
    }
}
