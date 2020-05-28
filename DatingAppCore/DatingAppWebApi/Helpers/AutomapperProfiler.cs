using System.Linq;
using AutoMapper;
using DatingAppDal.Model;
using DatingAppWebApi.Dto;

namespace DatingAppWebApi.Helpers
{
    public class AutomapperProfiler : Profile
    {
        public AutomapperProfiler()
        {
            CreateMap<User, UserListDto>()
                .ForMember(Photo => Photo.photoUrl, map =>
                map.MapFrom(x => x.Photos.FirstOrDefault(y => y.IsMain).Url
                ))
            .ForMember(age => age.age, map => map.ResolveUsing(
                  x => x.DateOfBirth.CalculateAge()
              )
            );
            CreateMap<User, UserDetailsDto>().ForMember(Photo => Photo.PhotoUrl, map => map.MapFrom(
                 x => x.Photos.FirstOrDefault(y => y.IsMain).Url
             )).ForMember(age => age.Age, map => map.ResolveUsing(
                   x => x.DateOfBirth.CalculateAge()
               )
            );
            CreateMap<Photo, PhotoDetailsDto>();
            CreateMap<UserEditDto, User>();
            CreateMap<Photo, PhotoToReturnDto>();
            CreateMap<PhotoCreationDto, Photo>();
            CreateMap<UserRegistrationDto, User>();
            
        }
    }
}