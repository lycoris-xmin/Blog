using Newtonsoft.Json;

namespace Lycoris.Blog.Core.Github.Models
{
    public class GithubPutFileResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public GithubPutFileContent? Content { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public GithubCommit? Commit { get; set; }
    }

    public class GithubPutFileContent
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Path { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Sha { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? Size { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("html_url ")]
        public string? HtmlUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("git_url ")]
        public string? GitUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("download_url ")]
        public string? DownloadUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("Type ")]
        public string? Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("_links ")]
        public GitHubPutFileLinks? Links { get; set; }
    }

    public class GitHubPutFileLinks
    {
        /// <summary>
        /// 
        /// </summary>
        public string? Self { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Git { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Html { get; set; }
    }
}
