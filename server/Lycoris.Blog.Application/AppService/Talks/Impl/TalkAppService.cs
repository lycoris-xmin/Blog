using Lycoris.Autofac.Extensions;
using Lycoris.AutoMapper.Extensions;
using Lycoris.Base.Extensions;
using Lycoris.Base.Helper;
using Lycoris.Blog.Application.AppService.Talks.Dtos;
using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.Core.EntityFrameworkCore;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.Model.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Lycoris.Blog.Application.AppService.Talks.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true)]
    public class TalkAppService : ApplicationBaseService, ITalkAppService
    {
        private readonly IRepository<Talk, long> _talk;

        public TalkAppService(IRepository<Talk, long> talk)
        {
            _talk = talk;
        }

        #region ======== 博客网站 ========
        public async Task<PageResultDto<TalkDataDto>> GetTalkListAsync(int pageIndex, int pageSize)
        {
            var count = await _talk.GetAll().CountAsync();
            if (count == 0 || !CheckPageFilter(pageIndex, pageSize, count))
                return new PageResultDto<TalkDataDto>(count);

            var query = _talk.GetAll()
                            .OrderByDescending(x => x.CreateTime)
                            .PageBy(pageIndex, pageSize)
                            .Select(x => new TalkDataDto()
                            {
                                Id = x.Id,
                                Content = x.Content,
                                AgentFlag = x.AgentFlag,
                                IpAddress = x.IpAddress,
                                CreateTime = x.CreateTime
                            });

            var list = await query.ToListAsync();

            return new PageResultDto<TalkDataDto>(count, list);
        }
        #endregion

        #region ======== 管理后台 ========
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<PageResultDto<MasterTalkDataDto>> GetListAsync(int pageIndex, int pageSize)
        {
            var count = await _talk.GetAll().CountAsync();
            if (count == 0 || !CheckPageFilter(pageIndex, pageSize, count))
                return new PageResultDto<MasterTalkDataDto>(count);

            var query = _talk.GetAll()
                             .OrderByDescending(x => x.CreateTime)
                             .PageBy(pageIndex, pageSize)
                             .Select(x => new MasterTalkDataDto()
                             {
                                 Id = x.Id,
                                 Content = x.Content,
                                 CreateTime = x.CreateTime
                             });

            var list = await query.ToListAsync();

            return new PageResultDto<MasterTalkDataDto>(count, list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<MasterTalkDataDto> CreateOrUpdateAsync(string content, long id = 0) => id > 0 ? UpdateTalkAsync(id, content) : CreateTalkAsync(content);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeletaAsync(long id)
        {
            var data = await _talk.GetAsync(id);
            if (data == null)
                return;

            await _talk.DeleteAsync(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private async Task<MasterTalkDataDto> CreateTalkAsync(string content)
        {
            var data = new Talk()
            {
                Content = content,
                UserAgent = CurrentRequest.UserAgent,
                AgentFlag = UserAgentHelper.GetUserAgent(CurrentRequest.UserAgent)?.Code ?? 0,
                Ip = IPAddressHelper.Ipv4ToUInt32(CurrentRequest.RequestIP),
                IpAddress = IPAddressHelper.ChangeAddress(IPAddressHelper.Search(CurrentRequest.RequestIP))
            };

            data = await _talk.CreateAsync(data);

            return data.ToMap<MasterTalkDataDto>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        private async Task<MasterTalkDataDto> UpdateTalkAsync(long id, string content)
        {
            var data = await _talk.GetAsync(id) ?? throw new FriendlyException("");

            var fieIds = new List<Expression<Func<Talk, object>>>();

            data.Content = content;
            fieIds.Add(x => x.Content!);

            if (data.UserAgent != CurrentRequest.UserAgent)
            {
                data.UserAgent = CurrentRequest.UserAgent;
                data.AgentFlag = UserAgentHelper.GetUserAgent(CurrentRequest.UserAgent)?.Code ?? 0;

                fieIds.Add(x => x.UserAgent!);
                fieIds.Add(x => x.AgentFlag!);
            }

            await _talk.UpdateFieIdsAsync(data, fieIds);

            return data.ToMap<MasterTalkDataDto>();
        }
        #endregion
    }
}
