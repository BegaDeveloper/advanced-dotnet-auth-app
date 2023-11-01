using AuthApp.Dto;
using AuthApp.Models;
using AutoMapper;

namespace AuthApp.Helper
{
    public class Mapper : Profile
    {
        public Mapper() {
            CreateMap<User, UserRegisterDto>();
            CreateMap<User, UserLoginDto>();
            CreateMap<UserRegisterDto, User>();
            CreateMap<UserLoginDto, User>();
        }

    }
}
