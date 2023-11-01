using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.EntityFrameworkCore.Constants;

namespace Lycoris.Blog.Application.AppServices.Comment.Dtos
{
    public class PostCommentDataDto
    {
        public long Id { get; set; }

        public UserInfoDto? User { get; set; }

        public long RepliedUserId { get; set; }

        public string? RepliedUser { get; set; }

        public string? Content { get; set; }

        public int? AgentFlag { get; set; }

        public string? IpAddress { get; set; }

        public DateTime CreateTime { get; set; }

        public bool IsOwner { get => User != null && User.Id == TableSeedData.UserData.Id; }
    }
}
