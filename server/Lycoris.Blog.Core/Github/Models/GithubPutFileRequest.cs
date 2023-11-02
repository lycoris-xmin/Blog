namespace Lycoris.Blog.Core.Github.Models
{
    public class GithubPutFileRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public GithubCommitter Committer { get; set; } = new GithubCommitter();

        /// <summary>
        /// 
        /// </summary>
        public string? Content { get; set; }
    }
}
