using Lycoris.Common.Extensions;

namespace Lycoris.Blog.Model.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class PostSettingConfiguration
    {
        /// <summary>
        /// 编辑器自动保存
        /// </summary>
        public bool AutoSave { get; set; } = false;

        /// <summary>
        /// 编辑器自动保存间隔
        /// </summary>
        public int Second { get; set; } = 30;

        /// <summary>
        /// 博客文章随机图
        /// </summary>
        public List<string> Images { get; set; } = new List<string>();

        /// <summary>
        /// 评论频率(单位秒)
        /// </summary>
        public int CommentFrequencySecond { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetRandomImage() => this.Images.HasValue() ? this.Images[new Random().Next(0, this.Images.Count - 1)] : "";
    }
}
