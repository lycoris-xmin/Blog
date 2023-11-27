using Lycoris.Blog.Application.Shared.Dtos;

namespace Lycoris.Blog.Application.AppServices.Message.Dtos
{
    public class WebMessageReplyListFilter : PageFilter
    {
        public int MessageId { get; set; }
    }
}
