using Lycoris.Blog.Application.Shared.Dtos;

namespace Lycoris.Blog.Application.AppServices.LeaveMessages.Dtos
{
    public class WebMessageReplyListFilter : PageFilter
    {
        public int MessageId { get; set; }
    }
}
