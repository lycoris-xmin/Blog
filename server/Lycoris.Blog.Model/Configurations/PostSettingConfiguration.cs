using Lycoris.Common.Extensions;

namespace Lycoris.Blog.Model.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class PostSettingConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        public bool AutoSave { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public int Second { get; set; } = 30;

        /// <summary>
        /// 
        /// </summary>
        public List<string> Images { get; set; } = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetRandomImage() => this.Images.HasValue() ? this.Images[new Random().Next(0, this.Images.Count - 1)] : "";
    }
}
