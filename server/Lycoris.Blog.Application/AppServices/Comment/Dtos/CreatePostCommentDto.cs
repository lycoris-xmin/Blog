namespace Lycoris.Blog.Application.AppServices.Comment.Dtos
{
    public class CreatePostCommentDto
    {
        /// <summary>
        /// 
        /// </summary>
        public long PostId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long? CommentId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public long? RepliedUserId { get; set; }
    }
}


