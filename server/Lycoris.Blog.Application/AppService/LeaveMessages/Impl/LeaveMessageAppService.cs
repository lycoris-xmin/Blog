using Lycoris.Autofac.Extensions;
using Lycoris.Base.Extensions;
using Lycoris.Base.Helper;
using Lycoris.Base.Utils.SensitiveWord;
using Lycoris.Blog.Application.AppService.LeaveMessages.Dtos;
using Lycoris.Blog.Application.Cached.ScheduleQueueCache;
using Lycoris.Blog.Application.Cached.ScheduleQueueCache.Dtos;
using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.Application.Shared.Impl;
using Lycoris.Blog.Core.EntityFrameworkCore;
using Lycoris.Blog.EntityFrameworkCore.Tables;
using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;
using Lycoris.Blog.Model.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Lycoris.Blog.Application.AppService.LeaveMessages.Impl
{
    [AutofacRegister(ServiceLifeTime.Scoped, PropertiesAutowired = true)]
    public class LeaveMessageAppService : ApplicationBaseService, ILeaveMessageAppService
    {
        private readonly IRepository<LeaveMessage, int> _message;
        private readonly Lazy<IRepository<User, long>> _user;
        private readonly Lazy<IScheduleQueueCacheService> _scheduleQueue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="user"></param>
        /// <param name="scheduleQueue"></param>
        public LeaveMessageAppService(IRepository<LeaveMessage, int> message, Lazy<IRepository<User, long>> user, Lazy<IScheduleQueueCacheService> scheduleQueue)
        {
            _message = message;
            _user = user;
            _scheduleQueue = scheduleQueue;
        }

        #region ======== 博客网站 ========
        /// <summary>
        /// 获取一级留言列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<PageResultDto<WebMessageDataDto>> GetWebMessageListAsync(int pageIndex, int pageSize)
        {
            var filter = _message.GetAll().Where(x => x.ParentId == 0).Where(x => x.Status != LeaveMessageStatusEnum.UserDelete);
            var count = await filter.CountAsync();
            if (count == 0 || !CheckPageFilter(pageIndex, pageSize, count))
                return new PageResultDto<WebMessageDataDto>(count);

            filter = filter.OrderByDescending(x => x.CreateTime).PageBy(pageIndex, pageSize);

            var query = filter.Select(x => new WebMessageDataDto()
            {
                Id = x.Id,
                User = new UserInfoDto() { Id = x.CreateUserId },
                Content = x.Content,
                AgentFlag = x.AgentFlag,
                ReplyCount = x.ReplyCount,
                IpAddress = x.IpAddress,
                Status = x.Status,
                CreateTime = x.CreateTime,
                RedundancyStr = x.Redundancy
            });

            var list = await query.ToListAsync();

            list = await GetMessageRedundancyAsync(list);

            list = await GetMessageUserInfoAsync(list);

            return new PageResultDto<WebMessageDataDto>(count, list);
        }

        /// <summary>
        /// 获取二级留言列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<List<WebMessageReplyDataDto>> GetWebMessageReplyListAsync(WebMessageReplyListFilter input)
        {
            var filter = _message.GetAll().Where(x => x.ParentId == input.MessageId)
                                          .Where(x => x.Status != LeaveMessageStatusEnum.UserDelete);

            var query = filter.OrderByDescending(x => x.CreateTime)
                              .PageBy(input.PageIndex, input.PageSize)
                              .Select(x => new WebMessageReplyDataDto()
                              {
                                  Id = x.Id,
                                  Content = x.Content,
                                  User = new UserInfoDto() { Id = x.CreateUserId },
                                  RepliedUser = x.RepliedUserId > 0 ? new LeaveMessageRepliedUserDto() { Id = x.RepliedUserId } : null,
                                  IpAddress = x.IpAddress,
                                  Status = x.Status,
                                  CreateTime = x.CreateTime
                              });

            var list = await query.ToListAsync();

            if (!list.HasValue())
                return new List<WebMessageReplyDataDto>();

            var userIds = list.Select(x => x.User.Id).ToList();
            userIds.AddRange(list.Where(x => x.RepliedUser != null).Select(x => x.RepliedUser!.Id).ToList());
            userIds = userIds.Where(x => x > 0).Distinct().ToList();

            var users = await _user.Value.GetAll()
                                   .Where(x => userIds.Contains(x.Id))
                                   .Select(x => new UserInfoDto()
                                   {
                                       Id = x.Id,
                                       NickName = x.NickName,
                                       Avatar = x.Avatar
                                   }).ToListAsync();

            if (!users.HasValue())
            {
                list.ForEach(x =>
                {
                    x.User = new UserInfoDto()
                    {
                        Id = 0,
                        NickName = "用户已注销",
                        Avatar = ""
                    };

                    if (x.RepliedUser != null)
                    {
                        x.RepliedUser = new LeaveMessageRepliedUserDto()
                        {
                            Id = 0,
                            NickName = "用户已注销",
                            Avatar = ""
                        };
                    }
                });

                return list;
            }

            foreach (var item in list)
            {
                item.User = users.SingleOrDefault(x => x.Id == item.User.Id) ?? new UserInfoDto()
                {
                    Id = 0,
                    NickName = "用户已注销",
                    Avatar = ""
                };

                if (item.RepliedUser != null)
                {
                    item.RepliedUser = users.Where(x => x.Id == item.RepliedUser.Id).Select(x => new LeaveMessageRepliedUserDto()
                    {
                        Id = x.Id,
                        NickName = x.NickName,
                        Avatar = x.Avatar
                    }).SingleOrDefault() ?? new LeaveMessageRepliedUserDto()
                    {
                        Id = 0,
                        NickName = "用户已注销",
                        Avatar = ""
                    };
                }
            }

            return list;
        }

        /// <summary>
        /// 发布一级留言
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<WebMessageDataDto> PublishMessageAsync(string content)
        {
            var data = new LeaveMessage()
            {
                Content = SensitiveWordMemoryStore.SensitiveWordsReplace(content),
                AgentFlag = UserAgentHelper.GetUserAgent(CurrentRequest.UserAgent)?.Code ?? 0,
                Ip = IPAddressHelper.Ipv4ToUInt32(CurrentRequest.RequestIP),
                IpAddress = IPAddressHelper.ChangeAddress(IPAddressHelper.Search(CurrentRequest.RequestIP)),
                ReplyCount = 0,
                Status = LeaveMessageStatusEnum.Default,
                CreateUserId = CurrentUser!.Id,
                CreateTime = DateTime.Now
            };

            if (data.Content != content)
                data.OriginalContent = content;

            data = await _message.CreateAsync(data);

            return new WebMessageDataDto()
            {
                Id = data.Id,
                User = new UserInfoDto()
                {
                    Id = CurrentUser!.Id,
                    NickName = CurrentUser.NickName,
                    Avatar = CurrentUser.Avatar
                },
                Content = data.Content,
                AgentFlag = data.AgentFlag,
                IpAddress = data.IpAddress,
                ReplyCount = data.ReplyCount,
                CreateTime = data.CreateTime,
                Status = data.Status,
            };
        }

        /// <summary>
        /// 发布二级留言
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        /// <exception cref="FriendlyException"></exception>
        public async Task<WebMessageReplyDataDto> PublishReplyMessageAsync(int messageId, string content)
        {
            var message = await _message.GetAsync(messageId);
            if (message == null)
                throw new FriendlyException("");
            else if (message.Status == LeaveMessageStatusEnum.Violation)
                throw new FriendlyException("该留言已违规，暂时无法回复");
            else if (message.Status == LeaveMessageStatusEnum.UserDelete)
                throw new FriendlyException("该留言已被用户自己删除，无法回复");

            var data = new LeaveMessage()
            {
                ParentId = message.ParentId > 0 ? message.ParentId : message.Id,
                RepliedUserId = message.ParentId > 0 && message.CreateUserId != CurrentUser!.Id ? message.CreateUserId : 0,
                Content = SensitiveWordMemoryStore.SensitiveWordsReplace(content),
                AgentFlag = UserAgentHelper.GetUserAgent(CurrentRequest.UserAgent)?.Code ?? 0,
                Ip = IPAddressHelper.Ipv4ToUInt32(CurrentRequest.RequestIP),
                IpAddress = IPAddressHelper.ChangeAddress(IPAddressHelper.Search(CurrentRequest.RequestIP)),
                ReplyCount = 0,
                Status = LeaveMessageStatusEnum.Default,
                CreateUserId = CurrentUser!.Id,
                CreateTime = DateTime.Now
            };

            if (data.Content != content)
                data.OriginalContent = content;

            data = await _message.CreateAsync(data);

            await _scheduleQueue.Value.EnqueueAsync(ScheduleTypeEnum.LeaveMessage, data.ParentId.ToString());

            var res = new WebMessageReplyDataDto()
            {
                Id = data.Id,
                User = new UserInfoDto()
                {
                    Id = CurrentUser!.Id,
                    NickName = CurrentUser.NickName,
                    Avatar = CurrentUser.Avatar
                },
                RepliedUser = data.RepliedUserId > 0 ? new LeaveMessageRepliedUserDto() { Id = data.RepliedUserId } : null,
                Content = data.Content,
                IpAddress = data.IpAddress,
                Status = data.Status,
                CreateTime = data.CreateTime,
            };

            if (res.RepliedUser != null)
            {
                res.RepliedUser = await _user.Value.GetSelectAsync(res.RepliedUser.Id, x => new LeaveMessageRepliedUserDto()
                {
                    Id = x.Id,
                    NickName = x.NickName,
                    Avatar = x.Avatar
                }) ?? new LeaveMessageRepliedUserDto()
                {
                    Id = 0,
                    NickName = "用户已注销",
                    Avatar = ""
                };
            }

            return res;
        }

        /// <summary>
        /// 删除自己的留言
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteSelfMessageAsync(int id)
        {
            if (id <= 0)
                throw new HttpStatusException(HttpStatusCode.BadRequest, "");

            var data = await _message.GetAsync(id) ?? throw new FriendlyException("");

            if (data.CreateUserId != CurrentUser!.Id)
                throw new HttpStatusException(HttpStatusCode.BadRequest, "");

            data.Status = LeaveMessageStatusEnum.UserDelete;
            await _message.UpdateFieIdsAsync(data, x => x.Status);

            if (data.ParentId == 0)
                return;

            await _scheduleQueue.Value.EnqueueAsync(ScheduleTypeEnum.LeaveMessage, data.ParentId.ToString());
        }

        /// <summary>
        /// 获取一级留言的下的二级留言展示
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private async Task<List<WebMessageDataDto>> GetMessageRedundancyAsync(List<WebMessageDataDto> list)
        {
            var tmp = list.Where(x => !x.RedundancyStr.IsNullOrEmpty()).Select(x => x.RedundancyStr.Split(",")).SelectMany(x => x).Select(x => x.ToInt()).ToList();
            if (!tmp.HasValue())
                return list;

            var filter = _message.GetAll().Where(x => tmp.Contains(x.Id));

            var query = filter.Select(x => new WebMessageReplyDataDto()
            {
                Id = x.Id,
                ParentId = x.ParentId,
                User = new UserInfoDto() { Id = x.CreateUserId },
                RepliedUser = x.RepliedUserId > 0 ? new LeaveMessageRepliedUserDto() { Id = x.RepliedUserId } : null,
                Content = x.Content,
                IpAddress = x.IpAddress,
                Status = x.Status,
                CreateTime = x.CreateTime
            });

            var replyList = await query.ToListAsync();

            foreach (var item in list)
            {
                var redundancy = replyList.Where(x => x.ParentId == item.Id).OrderByDescending(x => x.CreateTime).ToList();
                item.Redundancy = redundancy.HasValue() ? redundancy : null;
            }

            return list;
        }

        /// <summary>
        /// 获取留言的用户信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private async Task<List<WebMessageDataDto>> GetMessageUserInfoAsync(List<WebMessageDataDto> list)
        {
            var userIds = list.Select(x => x.User.Id).ToList();
            var redundancy = list.Where(x => x.Redundancy.HasValue());
            userIds.AddRange(redundancy.SelectMany(x => x.Redundancy!.Select(x => x.User.Id)).ToList() ?? new List<long>());
            userIds.AddRange(redundancy.SelectMany(x => x.Redundancy!.Where(x => x.RepliedUser != null).Select(x => x.RepliedUser!.Id)).ToList() ?? new List<long>());
            userIds = userIds.Where(x => x > 0).Distinct().ToList();

            var users = userIds.HasValue() ? await _user.Value.GetAll().Where(x => userIds.Contains(x.Id)).Select(x => new UserInfoDto { Id = x.Id, NickName = x.NickName, Avatar = x.Avatar }).ToListAsync() : new List<UserInfoDto>();

            if (!users.HasValue())
            {
                foreach (var item in list)
                {
                    item.User = new UserInfoDto()
                    {
                        Id = 0,
                        NickName = "用户已注销",
                        Avatar = ""
                    };

                    if (item.Redundancy.HasValue())
                    {
                        item.Redundancy!.ForEach(x =>
                        {
                            x.User = new UserInfoDto()
                            {
                                Id = 0,
                                NickName = "用户已注销",
                                Avatar = ""
                            };

                            if (x.RepliedUser != null)
                            {
                                x.RepliedUser = new LeaveMessageRepliedUserDto()
                                {
                                    Id = 0,
                                    NickName = "用户已注销",
                                    Avatar = ""
                                };
                            }
                        });
                    }
                }

                return list;
            }

            foreach (var item in list)
            {
                item.User = users!.SingleOrDefault(x => x.Id == item.User.Id) ?? new UserInfoDto()
                {
                    Id = 0,
                    NickName = "用户已注销",
                    Avatar = ""
                };

                if (item.Redundancy.HasValue())
                {
                    foreach (var reply in item.Redundancy!)
                    {
                        reply.User = users!.SingleOrDefault(x => x.Id == item.User.Id) ?? new UserInfoDto()
                        {
                            Id = 0,
                            NickName = "用户已注销",
                            Avatar = ""
                        };

                        if (reply.RepliedUser != null)
                        {
                            reply.RepliedUser = users!.Where(x => x.Id == item.User.Id).Select(x => new LeaveMessageRepliedUserDto()
                            {
                                Id = x.Id,
                                NickName = x.NickName,
                                Avatar = x.Avatar
                            }).SingleOrDefault() ?? new LeaveMessageRepliedUserDto()
                            {
                                Id = 0,
                                NickName = "用户已注销",
                                Avatar = ""
                            };
                        }
                    }
                }
            }

            return list;
        }
        #endregion

        #region ======== 管理后台 ========
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PageResultDto<MessageDataDto>> GetListAsync(MessageLsitFilter input)
        {
            var filter = _message.GetAll()
                                 .WhereIf(input.BeginTime.HasValue, x => x.CreateTime >= input.BeginTime)
                                 .WhereIf(input.EndTime.HasValue, x => x.CreateTime <= input.EndTime)
                                 .WhereIf(input.Ip.HasValue && input.Ip > 0, x => x.Ip == input.Ip!.Value)
                                 .WhereIf(input.Status.HasValue, x => x.Status == input.Status!.Value)
                                 .WhereIf(!input.Content.IsNullOrEmpty(), x => EF.Functions.Like(x.Content, $"%{input.Content}%"));

            var count = await filter.CountAsync();
            if (count == 0 || !CheckPageFilter(input, count))
                return new PageResultDto<MessageDataDto>(count);

            var query = from message in filter.OrderByDescending(x => x.CreateTime).PageBy(input.PageIndex, input.PageSize)

                        join u in _user.Value.GetAll() on message.CreateUserId equals u.Id into uu
                        from user in uu.DefaultIfEmpty()

                        select new MessageDataDto()
                        {
                            Id = message.Id,
                            Content = message.Content,
                            OriginalContent = message.OriginalContent,
                            Ip = message.Ip,
                            IpAddress = message.IpAddress,
                            Status = message.Status,
                            CreateUserId = user != null ? user.Id : 0,
                            CreateUser = user != null ? user.NickName : "用户已注销",
                            CreateTime = message.CreateTime,
                        };

            var list = await query.ToListAsync();

            return new PageResultDto<MessageDataDto>(count, list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task SetViolationMessageAsync(int id)
        {
            var data = await _message.GetAsync(id) ?? throw new FriendlyException("");

            if (data.Status == LeaveMessageStatusEnum.Violation)
                return;

            data.Status = LeaveMessageStatusEnum.Violation;
            await _message.UpdateFieIdsAsync(data, x => x.Status);

            if (data.ParentId > 0)
                await _scheduleQueue.Value.EnqueueAsync(ScheduleTypeEnum.LeaveMessage, data.ParentId.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task DeleteAsync(params int[] ids)
        {
            if (!ids.HasValue())
                return;

            var parentIds = await _message.GetAll().Where(x => ids.Contains(x.Id)).Where(x => x.ParentId > 0).Select(x => x.ParentId).ToListAsync();

            var sql = $"DELETE FROM {_message.TableName} WHERE Id IN ({string.Join(",", ids)});";
            await _message.ExecuteNonQueryAsync(sql);

            if (parentIds.HasValue())
            {
                sql = $"DELETE FROM {_message.TableName} WHERE ParentId IN ({string.Join(",", parentIds)})";
                await _message.ExecuteNonQueryAsync(sql);

                foreach (var item in parentIds)
                    await _scheduleQueue.Value.EnqueueAsync(ScheduleTypeEnum.LeaveMessage, item.ToString());
            }
        }
        #endregion
    }
}
