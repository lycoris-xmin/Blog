namespace Lycoris.Blog.Server.Models.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public class AdminViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public AdminViewModel()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path"></param>
        public AdminViewModel(string? Path)
        {
            this.Path = Path;
        }

        /// <summary>
        /// 
        /// </summary>
        public string? Path { get; set; }
    }
}
