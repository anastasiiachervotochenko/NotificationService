using AutoMapper;
using NotificationService.Data.Entity;
using NotificationService.Domain.Contracts.Models.DomainModels;
using NotificationService.Domain.Contracts.Models.RequestModel;
using NotificationService.Domain.Contracts.Models.Result;

namespace NotificationService.Mapping;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<User, UserModel>().ReverseMap();
        CreateMap<UserConfig, UserConfigModel>().ReverseMap();
        CreateMap<NotificationStatus, NotificationStatusModel>().ReverseMap();
        CreateMap<Notification, NotificationModel>().ReverseMap();
        CreateMap<Notification, NotificationResponseModel>().ReverseMap();
        CreateMap<CreateUserRequestModel, User>()
            .ForMember(u => u.Id, opt => opt.MapFrom(cu => Guid.NewGuid().ToString()));
        CreateMap<CreateNotificationRequestModel, Notification>()
            .ForMember(n => n.Id, opt => opt.MapFrom(cn => Guid.NewGuid().ToString()));
    }
}