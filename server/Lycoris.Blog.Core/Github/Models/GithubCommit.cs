using Newtonsoft.Json;

namespace Lycoris.Blog.Core.Github.Models
{
    public class GithubCommit
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Sha { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("node_id")]
        public string? NodeId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("html_url")]
        public string? HtmlUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public GithubAuthor? Author { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public GithubCommitter? Committer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public GithubTree? Tree { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<GithubParentsItem>? Parents { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public GithubVerification? Verification { get; set; }
    }
}
