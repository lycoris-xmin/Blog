using Lycoris.Blog.Model.Configurations;

namespace Lycoris.Blog.Server.Models.Home
{
    /// <summary>
    /// 
    /// </summary>
    public class AboutMeViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public AboutMeInfoViewModel? Info { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public AboutMeSkillConfiguration? Skill { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AboutMeInfoViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string[]? Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Educational { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? Institutions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Sex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string[]? Code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string[]? Hobby { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string[]? Introduction { get; set; }
    }
}
