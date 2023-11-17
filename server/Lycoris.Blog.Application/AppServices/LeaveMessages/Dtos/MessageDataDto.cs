using Lycoris.Blog.EntityFrameworkCore.Constants;
using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;

namespace Lycoris.Blog.Application.AppServices.LeaveMessages.Dtos
{
    public class MessageDataDto
    {
        public int Id { get; set; }

        public int ParentId { get; set; }

        public string? Content { get; set; }

        public string? OriginalContent { get; set; }

        public uint Ip { get; set; }

        public string? IpAddress { get; set; }

        public LeaveMessageStatusEnum? Status { get; set; }

        public long? CreateUserId { get; set; }

        public string? CreateUser { get; set; }

        public DateTime CreateTime { get; set; }

        public bool IsOwner { get => CreateUserId == TableSeedData.UserData.Id; }
    }
}
