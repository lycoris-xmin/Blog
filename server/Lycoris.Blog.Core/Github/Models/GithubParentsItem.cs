using Newtonsoft.Json;

namespace Lycoris.Blog.Core.Github.Models
{
    public class GithubParentsItem
    {
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
        public string? Sha { get; set; }
    }
}
