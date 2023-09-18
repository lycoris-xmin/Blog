using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;

namespace Lycoris.Blog.Application.AppService.LeaveMessages.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class WebMessageDataDto
    {
        public long Id { get; set; }

        public UserInfoDto User { get; set; } = new();

        public string Content { get; set; } = "";

        public int AgentFlag { get; set; }

        public int ReplyCount { get; set; }

        public List<WebMessageReplyDataDto>? Redundancy { get; set; }

        public string IpAddress { get; set; } = string.Empty;

        public LeaveMessageStatusEnum Status { get; set; }

        public DateTime CreateTime { get; set; }

        public bool? IsOwner { get => User.Id == TableSeedData.UserData.Id ? true : null; }

        public string RedundancyStr { get; set; } = "";
    }
}
