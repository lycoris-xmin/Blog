namespace Lycoris.Blog.Application.AppService.Categorys.Dtos
{
    public class CategoryDataDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Keyword { get; set; }

        public string? Icon { get; set; }

        public int? PostCount { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
