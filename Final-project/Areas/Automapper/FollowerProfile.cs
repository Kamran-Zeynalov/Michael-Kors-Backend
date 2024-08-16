using AutoMapper;
using Final_project.Areas.Admin.Model;
using Final_project.Entities;

namespace Final_project.Areas.Automapper
{
    public class FollowerProfile : Profile
    {
        public FollowerProfile()
        {
            CreateMap<Follower, FollowerViewModel>().ReverseMap();
        }
    }
}
