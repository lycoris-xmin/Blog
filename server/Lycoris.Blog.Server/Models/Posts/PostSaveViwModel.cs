namespace Lycoris.Blog.Server.Models.Posts
{
    /// <summary>
    /// 
    /// </summary>
    public class PostSaveViwModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public PostSaveViwModel(long id)
        {
            this.Id = id.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }
    }
}
