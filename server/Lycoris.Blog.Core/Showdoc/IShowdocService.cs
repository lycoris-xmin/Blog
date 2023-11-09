using Lycoris.Blog.Core.Showdoc.Models;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        Task PublishAsync(ShowdocTemplate template);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        Task PublishAsync(string host, string title, string content);
    }
}
