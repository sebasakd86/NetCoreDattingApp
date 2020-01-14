using System.Linq;
using AutoMapper;
using DattingApp.API.DTO;
using DattingApp.API.Model;

namespace DattingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            //Maps properties by name, otherwise we would've to configure it.
            CreateMap<User, UserForListDTO>()
                .ForMember(dest => dest.PhotoUrl,
                    opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.Age
                    , opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<User, UserForDetailedDTO>()
                .ForMember(dest => dest.PhotoUrl,
                    opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.Age
                    , opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotosForDetailDTO>();
            CreateMap<UserForUpdateDTO, User>();
            CreateMap<Photo, PhotoForReturnDTO>();
            CreateMap<PhotoForCreationDTO, Photo>();
            CreateMap<UserForRegisterDTO, User>();
            CreateMap<MessageForCreationDTO, Message>().ReverseMap();
            CreateMap<Message, MessageToReturnDTO>()
                .ForMember(m => m.SenderPhotoUrl, 
                    opt => opt.MapFrom(u => u.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(m => m.RecipientPhotoUrl, 
                    opt => opt.MapFrom(u => u.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url));
        }
    }
}