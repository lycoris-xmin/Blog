using Microsoft.AspNetCore.Http;

namespace Lycoris.Blog.Application.AppService.Categorys.Dtos
{
    public class CreateCategoryDto
    {
        public string? Name { get; set; }

        public string? Keyword { get; set; }

        public string? Icon { get; set; }

        public IFormFile? File { get; set; }
    }
}
