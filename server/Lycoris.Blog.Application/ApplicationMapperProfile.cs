using AutoMapper;
using Lycoris.Base.Extensions;
using Lycoris.Blog.Application.AppService.Authentication.Dtos;
using Lycoris.Blog.Application.AppService.Categorys.Dtos;
using Lycoris.Blog.Application.AppService.FriendLinks.Dtos;
using Lycoris.Blog.Application.AppService.Home.Dtos;
using Lycoris.Blog.Application.AppService.Posts.Dtos;
using Lycoris.Blog.Application.AppService.RequestLogs.Dtos;
using Lycoris.Blog.Application.AppService.SiteNavigations.Dtos;
using Lycoris.Blog.Application.AppService.Talks.Dtos;
using Lycoris.Blog.Application.Cached.AuthenticationCache.Dtos;
using Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Dtos;
using Lycoris.Blog.Application.SignalR.Dashboard.Dtos;
using Lycoris.Blog.Application.SignalR.Shared.Dtos;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Contexts;

namespace Lycoris.Blog.Application
{
    public class ApplicationMapperProfile : Profile
    {
        public ApplicationMapperProfile()
        {
            AllowNullCollections = true;

            CreateMap<User, LoginValidateDto>();

            CreateMap<LoginValidateDto, LoginDto>();

            CreateMap<LoginDto, LoginUserCacheDto>();

            CreateMap<Post, PostInfoDto>()
              .ForMember(x => x.Tags, opt => opt.MapFrom(src => src.Tags.ToObject<List<string>>() ?? new List<string>()));

            CreateMap<CreateCategoryDto, Category>().ForMember(x => x.Keyword, opt => opt.MapFrom(src => ChangeKeyword(src.Keyword)));

            CreateMap<Category, CategoryDataDto>();

            CreateMap<Post, PostDetailDto>()
                .ForMember(x => x.CategoryName, opt => opt.Ignore())
                .ForMember(x => x.PublishTime, opt => opt.MapFrom(src => src.UpdateTime));

            CreateMap<RequestLogQueueDto, RequestLog>()
                  .ForMember(x => x.Id, opt => opt.Ignore())
                  .ForMember(x => x.IP, opt => opt.Ignore())
                  .ForMember(x => x.IPAddress, opt => opt.Ignore());

            CreateMap<Talk, MasterTalkDataDto>();

            CreateMap<RequestLog, RequestLogInfoDto>();

            CreateMap<SignalRConnection, SignalRConnectionDto>();

            CreateMap<SignalRConnectionDto, SignalRConnection>().ForMember(x => x.Online, opt => opt.MapFrom(src => src.Online ?? false));

            CreateMap<FriendLinkApplyDto, FriendLink>()
                .ForMember(x => x.Status, opt => opt.MapFrom(src => false))
                .ForMember(x => x.CreateTime, opt => opt.MapFrom(src => DateTime.Now));

            CreateMap<SiteNavigation, SiteNavigationQueryDataDto>();

            CreateMap<User, WebOwnerDto>();

            CreateMap<RequestMonitorContext, RequestMonitorDto>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        private static string ChangeKeyword(string? keyword)
        {
            if (keyword.IsNullOrEmpty())
                return "";

            return keyword!.Replace("，", ",").Trim();
        }
    }
}
