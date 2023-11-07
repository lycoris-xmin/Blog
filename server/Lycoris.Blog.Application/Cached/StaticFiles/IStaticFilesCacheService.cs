namespace Lycoris.Blog.Application.Cached.StaticFiles
{
    public interface IStaticFilesCacheService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        void SetStaticFileUse(string fileName);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool GetStaticFileUse(string fileName);
    }
}
