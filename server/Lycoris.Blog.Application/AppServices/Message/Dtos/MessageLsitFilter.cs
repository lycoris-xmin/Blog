using Lycoris.Blog.Application.Shared.Dtos;
using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;

namespace Lycoris.Blog.Application.AppServices.Message.Dtos
{
    public class MessageLsitFilter : PageFilter
    {
        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string? Content { get; set; }

        public uint? Ip { get; set; }

        public LeaveMessageStatusEnum? Status { get; set; }
    }
}
