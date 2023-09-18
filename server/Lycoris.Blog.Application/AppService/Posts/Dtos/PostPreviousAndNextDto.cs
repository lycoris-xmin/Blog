namespace Lycoris.Blog.Application.AppService.Posts.Dtos
{
    public class PostPreviousAndNextDto
    {
        public PostPreviousAndNextDto()
        {

        }

        public PostPreviousAndNextDto(BlogPreviousAndNextDataDto? Previous, BlogPreviousAndNextDataDto? Next)
        {
            this.Previous = Previous;
            this.Next = Next;
        }

        public BlogPreviousAndNextDataDto? Previous { get; set; }

        public BlogPreviousAndNextDataDto? Next { get; set; }
    }

    public class BlogPreviousAndNextDataDto
    {
        public long Id { get; set; }

        public string? Title { get; set; }

        public string? Icon { get; set; }
    }
}
