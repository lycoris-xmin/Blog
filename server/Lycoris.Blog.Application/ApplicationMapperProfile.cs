using AutoMapper;
using Lycoris.Blog.Application.AppServices.Authentication.Dtos;
using Lycoris.Blog.Application.AppServices.Categorys.Dtos;
using Lycoris.Blog.Application.AppServices.Chat.Dtos;
using Lycoris.Blog.Application.AppServices.FriendLinks.Dtos;
using Lycoris.Blog.Application.AppServices.Home.Dtos;
using Lycoris.Blog.Application.AppServices.LoginFailedRecords.Dtos;
using Lycoris.Blog.Application.AppServices.Posts.Dtos;
using Lycoris.Blog.Application.AppServices.RequestLogs.Dtos;
using Lycoris.Blog.Application.AppServices.SiteNavigations.Dtos;
using Lycoris.Blog.Application.AppServices.Talks.Dtos;
using Lycoris.Blog.Application.Cached.Authentication.Models;
using Lycoris.Blog.Application.Cached.LoginFailedRecord.Models;
using Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Models;
using Lycoris.Blog.Application.SignalR.Models;
using Lycoris.Blog.Application.SignalR.Shared.Models;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Contexts;
using Lycoris.Common.Extensions;

namespace Lycoris.Blog.Application
{
    public class ApplicationMapperProfile : Profile
    {
        public ApplicationMapperProfile()
        {
            AllowNullCollections = true;

            CreateMap<User, LoginValidateDto>();

            CreateMap<LoginValidateDto, LoginDto>();

            CreateMap<LoginDto, LoginUserCacheModel>();

            CreateMap<Post, PostInfoDto>()
              .ForMember(x => x.Tags, opt => opt.MapFrom(src => src.Tags.ToObject<List<string>>() ?? new List<string>()));

            CreateMap<CreateCategoryDto, Category>().ForMember(x => x.Keyword, opt => opt.MapFrom(src => ChangeKeyword(src.Keyword)));

            CreateMap<Category, CategoryDataDto>();

            CreateMap<Post, PostDetailDto>()
                .ForMember(x => x.Browse, opt => opt.MapFrom(src => src.Statistics == null ? 0 : src.Statistics.Browse))
                .ForMember(x => x.CategoryName, opt => opt.Ignore())
                .ForMember(x => x.PublishTime, opt => opt.MapFrom(src => src.UpdateTime));

            CreateMap<RequestLogQueueModel, RequestLog>()
                  .ForMember(x => x.Id, opt => opt.Ignore())
                  .ForMember(x => x.IP, opt => opt.Ignore())
                  .ForMember(x => x.IPAddress, opt => opt.Ignore());

            CreateMap<Talk, MasterTalkDataDto>();

            CreateMap<RequestLog, RequestLogInfoDto>();

            CreateMap<FriendLinkApplyDto, FriendLink>()
                .ForMember(x => x.Status, opt => opt.MapFrom(src => false))
                .ForMember(x => x.CreateTime, opt => opt.MapFrom(src => DateTime.Now));

            CreateMap<SiteNavigation, SiteNavigationQueryDataDto>();

            CreateMap<User, WebOwnerDto>();

            CreateMap<LoginFailedRecordDto, LoginFailedRecordCacheModel>();

            CreateMap<SignalRConnection, SignalRConnectionModel>();

            CreateMap<SignalRConnectionModel, SignalRConnection>().ForMember(x => x.Online, opt => opt.MapFrom(src => src.Online ?? false));

            CreateMap<ChatMessageDataDto, ChatMessageSignalRModel>();

            CreateMap<RequestMonitorContext, RequestMonitorModel>();
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
