namespace Lycoris.Blog.Core.Showdoc
{
    public interface IShowdocService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        Task PublishAsync(string title, string content);
    }
}
