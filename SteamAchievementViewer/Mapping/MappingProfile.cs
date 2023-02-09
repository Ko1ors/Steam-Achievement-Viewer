using Sav.Infrastructure.Entities;
using SteamAchievementViewer.Models;
using SteamAchievementViewer.Services;
using System.Collections.Generic;
using Profile = AutoMapper.Profile;

namespace SteamAchievementViewer.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Models.Profile, UserEntity>()
                .ForMember(dest => dest.SteamID, opt => opt.MapFrom(src => src.SteamID))
                .ForMember(dest => dest.OnlineState, opt => opt.MapFrom(src => src.OnlineState))
                .ForMember(dest => dest.StateMessage, opt => opt.MapFrom(src => src.StateMessage))
                .ForMember(dest => dest.PrivacyState, opt => opt.MapFrom(src => src.PrivacyState))
                .ForMember(dest => dest.VisibilityState, opt => opt.MapFrom(src => src.VisibilityState))
                .ForMember(dest => dest.AvatarIcon, opt => opt.MapFrom(src => src.AvatarIcon))
                .ForMember(dest => dest.AvatarMedium, opt => opt.MapFrom(src => src.AvatarMedium))
                .ForMember(dest => dest.AvatarFull, opt => opt.MapFrom(src => src.AvatarFull))
                .ForMember(dest => dest.VacBanned, opt => opt.MapFrom(src => src.VacBanned))
                .ForMember(dest => dest.TradeBanState, opt => opt.MapFrom(src => src.TradeBanState))
                .ForMember(dest => dest.IsLimitedAccount, opt => opt.MapFrom(src => src.IsLimitedAccount))
                .ForMember(dest => dest.CustomURL, opt => opt.MapFrom(src => src.CustomURL))
                .ForMember(dest => dest.MemberSince, opt => opt.MapFrom(src => src.MemberSince))
                .ForMember(dest => dest.HoursPlayed2Wk, opt => opt.MapFrom(src => src.HoursPlayed2Wk))
                .ForMember(dest => dest.Headline, opt => opt.MapFrom(src => src.Headline))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.Realname, opt => opt.MapFrom(src => src.Realname))
                .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Summary))
                .ForMember(dest => dest.UserGames, opt => opt.Ignore())
                .ForMember(dest => dest.UserAchievements, opt => opt.Ignore())
                .ForMember(dest => dest.Inserted, opt => opt.Ignore())
                .ForMember(dest => dest.Updated, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Game, GameEntity>()
                .ForMember(dest => dest.AppID, opt => opt.MapFrom(src => src.AppID))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Logo, opt => opt.MapFrom(src => src.Logo))
                .ForMember(dest => dest.GameLogoSmall, opt => opt.MapFrom(src => src.GameLogoSmall))
                .ForMember(dest => dest.StoreLink, opt => opt.MapFrom(src => src.StoreLink))
                .ForMember(dest => dest.HoursLast2Weeks, opt => opt.MapFrom(src => src.HoursLast2Weeks))
                .ForMember(dest => dest.HoursOnRecord, opt => opt.MapFrom(src => src.HoursOnRecord))
                .ForMember(dest => dest.GlobalStatsLink, opt => opt.MapFrom(src => src.GlobalStatsLink))
                .ForMember(dest => dest.UserGames, opt => opt.Ignore())
                .ForMember(dest => dest.Achievements, opt => opt.Ignore())
                .ForMember(dest => dest.Inserted, opt => opt.Ignore())
                .ForMember(dest => dest.Updated, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<UserEntity, UserGameEntity>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.SteamID64))
                .ForMember(dest => dest.AppID, opt => opt.Ignore())
                .ForMember(dest => dest.StatsLink, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Game, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Game, UserGameEntity>()
                .ForMember(dest => dest.AppID, opt => opt.MapFrom(src => src.AppID))
                .ForMember(dest => dest.StatsLink, opt => opt.MapFrom(src => src.StatsLink))
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Game, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Inserted, opt => opt.Ignore())
                .ForMember(dest => dest.Updated, opt => opt.Ignore())
                .ForSourceMember(src => src.Achievements, opt => opt.DoNotValidate())
                .ReverseMap();
            
            CreateMap<UserEntity, List<UserGameEntity>>()
                .ConvertUsing<GenericSingleToListConverter<UserEntity, UserGameEntity>>();

            CreateMap<Achievement, AchievementEntity>()
                .ForMember(dest => dest.AppID, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Percent, opt => opt.MapFrom(src => src.Percent))
                .ForMember(dest => dest.IconOpen, opt => opt.MapFrom(src => src.IconOpen))
                .ForMember(dest => dest.IconClosed, opt => opt.MapFrom(src => src.IconClosed))
                .ForMember(dest => dest.Apiname, opt => opt.MapFrom(src => src.Apiname))
                .ForMember(dest => dest.Updated, opt => opt.Ignore())
                .ForMember(dest => dest.Inserted, opt => opt.Ignore())
                .ForMember(dest => dest.Updated, opt => opt.Ignore())
                .ForMember(dest => dest.Game, opt => opt.Ignore())
                .ForMember(dest => dest.UserAchievements, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<GameEntity, AchievementEntity>()
                .ForMember(dest => dest.AppID, opt => opt.MapFrom(src => src.AppID))
                .ForMember(dest => dest.Game, opt => opt.Ignore())
                .ForMember(dest => dest.UserAchievements, opt => opt.Ignore())
                 .ForMember(dest => dest.IconClosed, opt => opt.Ignore())
                .ForMember(dest => dest.IconOpen, opt => opt.Ignore())
                .ForMember(dest => dest.Apiname, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.Ignore())
                .ForMember(dest => dest.Percent, opt => opt.Ignore())
                .ReverseMap();


            CreateMap<GameEntity, List<AchievementEntity>>()
                .ConvertUsing<GenericSingleToListConverter<GameEntity, AchievementEntity>>();
        }
    }
}
