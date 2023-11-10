﻿using AutoMapper;
using Lycoris.Blog.Application.AppServices.AccessControls.Dtos;
using Lycoris.Blog.Application.AppServices.Authentication.Dtos;
using Lycoris.Blog.Application.AppServices.BrowseLogs.Dtos;
using Lycoris.Blog.Application.AppServices.Categorys.Dtos;
using Lycoris.Blog.Application.AppServices.Chat.Dtos;
using Lycoris.Blog.Application.AppServices.Comment.Dtos;
using Lycoris.Blog.Application.AppServices.Dashboard.Dtos;
using Lycoris.Blog.Application.AppServices.FriendLinks.Dtos;
using Lycoris.Blog.Application.AppServices.Home.Dtos;
using Lycoris.Blog.Application.AppServices.LeaveMessages.Dtos;
using Lycoris.Blog.Application.AppServices.Posts.Dtos;
using Lycoris.Blog.Application.AppServices.RequestLogs.Dtos;
using Lycoris.Blog.Application.AppServices.SiteNavigations.Dtos;
using Lycoris.Blog.Application.AppServices.StaticFiles.Dtos;
using Lycoris.Blog.Application.AppServices.Talks.Dtos;
using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.Application.SignalR.Models;
using Lycoris.Blog.Model.Configurations;
using Lycoris.Blog.Model.Global.Output;
using Lycoris.Blog.Server.Models.AccessControls;
using Lycoris.Blog.Server.Models.Authentication;
using Lycoris.Blog.Server.Models.BrowseLogs;
using Lycoris.Blog.Server.Models.Categorys;
using Lycoris.Blog.Server.Models.Chat;
using Lycoris.Blog.Server.Models.Comment;
using Lycoris.Blog.Server.Models.Configurations;
using Lycoris.Blog.Server.Models.Dashboard;
using Lycoris.Blog.Server.Models.FriendLinks;
using Lycoris.Blog.Server.Models.Home;
using Lycoris.Blog.Server.Models.LeaveMessages;
using Lycoris.Blog.Server.Models.Posts;
using Lycoris.Blog.Server.Models.RequestLogs;
using Lycoris.Blog.Server.Models.Shared;
using Lycoris.Blog.Server.Models.SiteNavigations;
using Lycoris.Blog.Server.Models.StaticFiles;
using Lycoris.Blog.Server.Models.Talks;
using Lycoris.Common.Extensions;
using Lycoris.Common.Helper;
using System.Text;

namespace Lycoris.Blog.Server.Application
{
    /// <summary>
    /// 
    /// </summary>
    public class ViewModelMapperProfile : Profile
    {
        /// <summary>
        /// 
        /// </summary>
        public ViewModelMapperProfile()
        {
            AllowNullCollections = true;

            CreateMap(typeof(EnumsDto<>), typeof(EnumsViewModel<>));

            CreateMap(typeof(PageResultDto<>), typeof(PageViewModel<>));

            CreateMap<LoginDto, LoginViewModel>();

            CreateMap<RegisterInput, RegisterDto>();

            CreateMap<NearlyDaysWebStatisticsDataDto, NearlyDaysWebStatisticsDataViewModel>();

            CreateMap<UserBriefDto, UserBriefViewModel>();

            CreateMap<StatisticsDto, ServerStatisticsViewModel>();

            CreateMap<PostQueryListInput, PostQueryFilter>().ForMember(x => x.IsPublish, opt => opt.MapFrom(src => IntChangeBool(src.IsPublish)));

            CreateMap<PostQueryDataDto, PostQueryDataViewModel>()
                .ForMember(x => x.Comment, opt => opt.MapFrom(src => src.Comment ? 1 : 0))
                .ForMember(x => x.IsPublish, opt => opt.MapFrom(src => src.IsPublish ? 1 : 0))
                .ForMember(x => x.Recommend, opt => opt.MapFrom(src => src.Recommend ? 1 : 0));

            CreateMap<PostInfoDto, PostInfoViewModel>().ForMember(x => x.Comment, opt => opt.MapFrom(src => src.Comment.HasValue && src.Comment.Value ? 1 : 0));

            CreateMap<PostSaveInput, PostSaveDto>().ForMember(x => x.Comment, opt => opt.MapFrom(src => src.Comment.HasValue && src.Comment.Value == 1));

            CreateMap<CategoryDataDto, CategoryDataViewModel>().ForMember(x => x.Keyword, opt => opt.MapFrom(src => ChangeKeywordList(src.Keyword)));

            CreateMap<CategoryCreateInput, CreateCategoryDto>()
                .ForMember(x => x.Keyword, opt => opt.MapFrom(src => ChangeKeyword(src.Keyword)))
                .ForMember(x => x.Icon, opt => opt.Ignore());

            CreateMap<CategoryUpdateInput, UpdateCategoryDto>()
                .ForMember(x => x.Keyword, opt => opt.MapFrom(src => ChangeKeyword(src.Keyword)))
                .ForMember(x => x.Icon, opt => opt.Ignore());


            CreateMap<PostListInput, PostFilter>();

            CreateMap<PostDataDto, PostDataViewModel>()
                .ForMember(x => x.Tags, opt => opt.MapFrom(src => ChangeBlogTags(src.Tags, null)))
                .ForMember(x => x.PublishTime, opt => opt.MapFrom(src => ChangeTimeToChinese(src.PublishTime)));

            CreateMap<PostDetailDto, PostDetailViewModel>()
                .ForMember(x => x.Category, opt => opt.MapFrom(src => src.CategoryName ?? ""))
                .ForMember(x => x.Tags, opt => opt.MapFrom(src => ChangeBlogTags(src.Tags, null)))
                .ForMember(x => x.PublishTime, opt => opt.MapFrom(src => src.PublishTime.ToString("yyyy-MM-dd")));

            CreateMap<SaveWebSettingsInput, WebSettingsConfiguration>();

            CreateMap<SaveEmailSettingsInput, EmailSettingsConfiguration>();

            CreateMap<SaveSeoSettingsInput, SeoSettingsConfiguration>();

            CreateMap<BaiduSeoSettings, BaiduSeoConfiguration>();

            CreateMap<UserBriefDto, UserBriefViewModel>();

            //CreateMap<ChangePasswordInput, ChangePasswordDto>();

            CreateMap<HomeCategoryDataDto, CategoryHeaderDataViewModel>();

            CreateMap<SaveStaticFileSettingsInput, StaticFileConfiguration>();

            CreateMap<PostRecommendDataDto, PostRecommendDataViewModel>();

            CreateMap<MasterTalkDataDto, MasterTalkDataViewModel>();

            CreateMap<TalkDataDto, TalkDataViewModel>().ForMember(x => x.CreateTime, opt => opt.MapFrom(src => ChangeTimeToChinese(src.CreateTime)));

            CreateMap<TalkCommentDataDto, TalkCommentDataViewModel>().ForMember(x => x.CreateTime, opt => opt.MapFrom(src => ChangeTimeToChinese(src.CreateTime)));

            CreateMap<PostCommentListInput, PostCommentListFilter>();

            CreateMap<PostCommentDataDto, PostCommentDataViewModel>().ForMember(x => x.CreateTime, opt => opt.MapFrom(src => ChangeTimeToChinese(src.CreateTime)));

            CreateMap<PublishCommentInput, CreatePostCommentDto>();

            CreateMap<RequestLogListInput, GetRequestLogListFilter>().ForMember(x => x.Ip, opt => opt.MapFrom(src => IPAddressHelper.Ipv4ToUInt32(src.Ip ?? "")));

            CreateMap<RequestLogDataDto, RequestLogDataViewModel>().ForMember(x => x.Ip, opt => opt.MapFrom(src => IPAddressHelper.UInt32ToIpv4(src.Ip)));

            CreateMap<RequestLogInfoDto, RequestLogInfoViewModel>();

            CreateMap<SavePostSettingsInput, PostSettingConfiguration>()
                .ForMember(x => x.Second, opt => opt.MapFrom(src => src.Second ?? 0))
                .ForMember(x => x.Images, opt => opt.MapFrom(src => src.Images ?? new List<string>()));

            CreateMap<WebOwnerDto, WebOwnerViewModel>()
                .ForMember(x => x.QQ, opt => opt.MapFrom(src => ChangeEmptyStringToNull(src.QQ)))
                .ForMember(x => x.Wechat, opt => opt.MapFrom(src => ChangeEmptyStringToNull(src.Wechat)))
                .ForMember(x => x.Github, opt => opt.MapFrom(src => ChangeEmptyStringToNull(src.Github)))
                .ForMember(x => x.CloudMusic, opt => opt.MapFrom(src => ChangeEmptyStringToNull(src.CloudMusic)))
                .ForMember(x => x.Bilibili, opt => opt.MapFrom(src => ChangeEmptyStringToNull(src.Bilibili)));

            CreateMap<AboutMeInfoConfiguration, AboutMeInfoViewModel>()
                .ForMember(x => x.Age, opt => opt.MapFrom(src => src.Birth.HasValue ? (DateTime.Now.Year - src.Birth.Value.Year) : 0));

            CreateMap<BlogPreviousAndNextDataDto, BlogPreviousAndNextDataViewModel>();

            CreateMap<PostPreviousAndNextDto, PostPreviousAndNextViewModel>();

            CreateMap<UserInfoDto, UserInfoViewModel>();

            CreateMap<PostCommentQueryListInput, PostCommentQueryListFilter>();

            CreateMap<PostCommentQueryDataDto, PostCommentQueryDataViewModel>();

            CreateMap<WebMessageDataDto, LeaveMessageDataViewModel>().ForMember(x => x.CreateTime, opt => opt.MapFrom(src => ChangeTimeToChinese(src.CreateTime)));

            CreateMap<LeaveMessageRepliedUserDto, LeaveMessageRepliedUserViewModel>();

            CreateMap<WebMessageReplyDataDto, LeaveMessageReplyDataViewModel>()
                .ForMember(x => x.CreateTime, opt => opt.MapFrom(src => ChangeTimeToChinese(src.CreateTime)));

            CreateMap<LeaveMessageReplyListInput, WebMessageReplyListFilter>();

            CreateMap<LeaveMessageQueryListInput, MessageLsitFilter>().ForMember(x => x.Ip, opt => opt.MapFrom(src => IPAddressHelper.Ipv4ToUInt32(src.Ip ?? "")));

            CreateMap<MessageDataDto, LeaveMessageQueryDataViewModel>()
                .ForMember(x => x.Ip, opt => opt.MapFrom(src => IPAddressHelper.UInt32ToIpv4(src.Ip)))
                .ForMember(x => x.OriginalContent, opt => opt.MapFrom(src => src.OriginalContent.IsNullOrEmpty() ? null : src.OriginalContent));

            CreateMap<SearchPostDataDto, SearchPostDataViewModel>();

            CreateMap<WebBrowseRecordInput, WebBrowseRecordDto>();

            CreateMap<BrowseLogListInput, BrowseLogListFilter>();

            CreateMap<BrowseLogDataDto, BrowseLogDataViewModel>().ForMember(x => x.Ip, opt => opt.MapFrom(src => IPAddressHelper.UInt32ToIpv4(src.Ip)));

            CreateMap<BrowseRefererDataDto, BrowseRefererDataViewModel>();

            CreateMap<NearlyDaysWebStatisticsDataDto, NearlyDaysWebStatisticsDataViewModel>().ForMember(x => x.Day, opt => opt.MapFrom(src => src.Day.ToString("yyyy-MM-dd")));

            CreateMap<RegisterInput, RegisterDto>();

            CreateMap<ChatRoomDto, ChatRoomViewModel>();

            CreateMap<ChatMessageListInput, GetChatMessageListFilter>();

            CreateMap<ChatMessageDataDto, ChatMessageDataViewModel>();

            CreateMap<ChatMessageDataDto, ChatMessageSignalRModel>();

            CreateMap<FriendLinkDataDto, FriendLinkDataViewModel>();

            CreateMap<FriendLinkApplyInput, FriendLinkApplyDto>();

            CreateMap<FriendLinkQueryListInput, FriendLinkQueryFilter>();

            CreateMap<FriendLinkQueryDataDto, FriendLinkQueryDataViewModel>();

            CreateMap<FriendLinkAudtInput, AuditFriendLinkDto>();

            CreateMap<FriendLinkCreateInput, CreateFriendLinkDto>();

            CreateMap<StatisticsDto, ServerStatisticsViewModel>();

            CreateMap<PostStatisticsDto, PostStatisticsViewModel>();

            CreateMap<SiteNavigationQueryListInput, SiteNavigationQueryFilter>();

            CreateMap<SiteNavigationQueryDataDto, SiteNavigationQueryDataViewModel>();

            CreateMap<SiteNavigationCreateInput, CreateSiteNavigationDto>();

            CreateMap<SiteNavigationUpdateInput, UpdateSiteNavigationDto>();

            CreateMap<SiteNavigationDomainDataDto, SiteNavigationDomainDataViewModel>();

            CreateMap<SiteNavigationDataDto, SiteNavigationDataViewModel>();

            CreateMap<StaticFileListInput, StaticFileListFilter>();

            CreateMap<StaticFileDataDto, StaticFileDataViewModel>();

            CreateMap<WebStatisticsDto, WebStatisticsViewModel>();

            CreateMap<SaveSystemFileClearConfigurationInput, SystemFileClearConfiguration>();

            CreateMap<SaveSystemDBClearConfigurationInput, SystemDBClearConfiguration>();

            CreateMap<AccessControlListInput, GetAccessControlListFilter>();

            CreateMap<AccessControlDataDto, AccessControlDataViewModel>()
                .ForMember(x => x.Ip, opt => opt.MapFrom(src => IPAddressHelper.UInt32ToIpv4(src.Ip)));
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        private static List<string> ChangeKeywordList(string? keyword) => keyword?.Split(',').Where(x => !x.IsNullOrEmpty()).ToList() ?? new List<string>();

        /// <summary>
        /// 隐藏敏感信息
        /// </summary>
        /// <param name="info">信息实体</param>
        /// <param name="left">左边保留的字符数</param>
        /// <param name="right">右边保留的字符数</param>
        /// <param name="basedOnLeft">当长度异常时，是否显示左边 </param>
        /// <returns></returns>
        private static string HideSensitiveInfo(string info, int left, int right, bool basedOnLeft = true)
        {
            if (string.IsNullOrEmpty(info))
                return "";

            var sbText = new StringBuilder();
            int hiddenCharCount = info.Length - left - right;
            if (hiddenCharCount > 0)
            {
                string prefix = info[..left], suffix = info[^right..];
                sbText.Append(prefix);
                for (int i = 0; i < hiddenCharCount; i++)
                {
                    sbText.Append('*');
                }
                sbText.Append(suffix);
            }
            else
            {
                if (basedOnLeft)
                {
                    if (info.Length > left && left > 0)
                    {
                        sbText.Append(info[..left] + "****");
                    }
                    else
                    {
                        sbText.Append(info[..1] + "****");
                    }
                }
                else
                {
                    if (info.Length > right && right > 0)
                    {
                        sbText.Append(string.Concat("****", info.AsSpan(info.Length - right)));
                    }
                    else
                    {
                        sbText.Append(string.Concat("****", info.AsSpan(info.Length - 1)));
                    }
                }
            }
            return sbText.ToString();
        }

        /// <summary>
        /// 隐藏敏感信息
        /// </summary>
        /// <param name="info">信息</param>
        /// <param name="sublen">信息总长与左子串（或右子串）的比例</param>
        /// <param name="basedOnLeft">当长度异常时，是否显示左边，默认true，默认显示左边</param>
        /// <code>true</code>显示左边，<code>false</code>显示右边
        /// <returns></returns>
        private static string HideSensitiveInfo(string info, int sublen = 3, bool basedOnLeft = true)
        {
            if (string.IsNullOrEmpty(info))
            {
                return "";
            }
            if (sublen <= 1)
            {
                sublen = 3;
            }
            int subLength = info.Length / sublen;
            if (subLength > 0 && info.Length > (subLength * 2))
            {
                string prefix = info[..subLength], suffix = info[^subLength..];
                return prefix + "****" + suffix;
            }
            else
            {
                if (basedOnLeft)
                {
                    string prefix = subLength > 0 ? info[..subLength] : info[..1];
                    return prefix + "****";
                }
                else
                {
                    string suffix = subLength > 0 ? info[^subLength..] : info[^1..];
                    return "****" + suffix;
                }
            }
        }

        /// <summary>
        /// 隐藏邮件详情
        /// </summary>
        /// <param name="email">邮件地址</param>
        /// <param name="left">邮件头保留字符个数，默认值设置为3</param>
        /// <returns></returns>
        private static string HideEmailDetails(string email, int left = 3)
        {
            if (string.IsNullOrEmpty(email))
            {
                return "";
            }
            if (System.Text.RegularExpressions.Regex.IsMatch(email, @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))//如果是邮件地址
            {
                int suffixLen = email.Length - email.LastIndexOf('@');
                return HideSensitiveInfo(email, left, suffixLen, false);
            }
            else
            {
                return HideSensitiveInfo(email);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private static bool? IntChangeBool(int? val)
        {
            if (!val.HasValue)
                return null;

            return val.Value == 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        private static List<string> ChangeBlogTags(string? tags, int? take)
        {
            if (tags.IsNullOrEmpty())
                return new List<string>();

            var tagList = tags.ToObject<List<string>>();

            if (!tagList.HasValue())
                return new List<string>();

            return take.HasValue ? tagList!.Take(take!.Value).ToList() : tagList!;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private static string ChangeTimeToChinese(DateTime dateTime)
        {
            var now = DateTime.Now;
            if (dateTime.AddMinutes(5) >= now)
                return "刚刚";
            else if (dateTime.AddHours(1) > now)
                return $"{(int)Math.Ceiling((now - dateTime).TotalMinutes)}分钟之前";
            else if (dateTime.ToString("yyyy-MM-dd") == now.ToString("yyyy-MM-dd"))
                return dateTime.ToString("HH:mm:ss");
            else
            {
                var days = (int)Math.Ceiling((now - dateTime).TotalDays);
                if (days <= 7)
                    return $"{days}天前";
                else if (dateTime.Year == now.Year)
                    return dateTime.ToString("MM-dd");
                else
                    return dateTime.ToString("yyyy-MM-dd");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private static string ChangeRoomLastActiveTime(DateTime dateTime)
        {
            if (dateTime.Year != DateTime.Now.Year)
                return dateTime.ToString("yyyy/m/d");
            else if (dateTime.Month != DateTime.Now.Month)
                return dateTime.ToString("M-d");
            else if (dateTime.Date != DateTime.Now.Date)
                return (DateTime.Now.Date - dateTime.Date).TotalDays == 1 ? "昨天" : dateTime.ToString("M-d");
            else
                return dateTime.ToString("HH:mm");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string? ChangeEmptyStringToNull(string? str) => str.IsNullOrEmpty() ? null : str;
    }
}
