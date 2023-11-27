using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;

namespace Lycoris.Blog.Application.AppServices.Message.Dtos
{
    public class WebMessageReplyDataDto
    {
        public long Id { get; set; }

        public int ParentId { get; set; }

        public UserInfoDto User { get; set; } = new();

        public LeaveMessageRepliedUserDto? RepliedUser { get; set; }

        public string Content { get; set; } = "";

        public string IpAddress { get; set; } = string.Empty;

        public LeaveMessageStatusEnum Status { get; set; }

        public DateTime CreateTime { get; set; }

        public bool? IsOwner { get => User.Id == TableSeedData.UserData.Id ? true : null; }
    }


    public class LeaveMessageRepliedUserDto : UserInfoDto
    {
        public bool? IsOwner { get => Id == TableSeedData.UserData.Id ? true : null; }
    }
}
