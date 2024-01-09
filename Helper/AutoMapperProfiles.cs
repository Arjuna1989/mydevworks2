using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Entity;
using API.Extension;
using AutoMapper;

namespace API.Helper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User,UserDto>()
                 .ForMember(dest=> dest.PhotoUrl , opt => opt.MapFrom(src =>src.Photos.FirstOrDefault(x=> x.IsMain).Url))
                 .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<Photos,PhotoDto>();
            CreateMap<UserUpdateDto,User>();
        }
    }
}