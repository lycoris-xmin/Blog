namespace Lycoris.Blog.Application.Shared.Dtos
{
    public class PageResultDto<T> where T : class
    {
        public PageResultDto()
        {
            this.Count = 0;
            this.Summary = null;
            this.List = new List<T>();
        }

        public PageResultDto(int Count, List<T>? List = null)
        {
            this.Count = Count;
            this.Summary = null;
            this.List = List ?? new List<T>();
        }

        public PageResultDto(int Count, T? Summary, List<T>? List = null)
        {
            this.Count = Count;
            this.Summary = Summary;
            this.List = List ?? new List<T>();
        }

        public int Count { get; set; }

        public T? Summary { get; set; }

        public List<T> List { get; set; }
    }
}
