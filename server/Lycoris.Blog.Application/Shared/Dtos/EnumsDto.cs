namespace Lycoris.Blog.Application.Shared.Dtos
{
    public class EnumsDto<T>
    {
        public T? Value { get; set; }

        public string? Name { get; set; }

        public string? Data { get; set; }
    }
}
