namespace Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Dtos
{
    public class PostStaticQueueDto
    {
        public PostStaticQueueDto() { }

        public PostStaticQueueDto(long PostId, PostStaticTypeEnum StaticType)
        {
            this.PostId = PostId;
            this.StaticType = StaticType;
        }

        public long PostId { get; set; }

        public PostStaticTypeEnum StaticType { get; set; }
    }

    public enum PostStaticTypeEnum
    {
        Browse = 0,
        Comment = 1
    }
}
