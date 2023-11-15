namespace Lycoris.Blog.Application.Schedule.JobServices.ScheduleQueue.Models
{
    internal class PostStaticQueueModel
    {
        public PostStaticQueueModel() { }

        public PostStaticQueueModel(long PostId, PostStaticTypeEnum StaticType)
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
