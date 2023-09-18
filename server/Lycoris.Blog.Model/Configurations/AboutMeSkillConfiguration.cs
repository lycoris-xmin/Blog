namespace Lycoris.Blog.Model.Configurations
{
    /// <summary>
    /// 
    /// </summary>
    public class AboutMeSkillConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        public List<AboutMeSkillData>? Web { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<AboutMeSkillData>? Server { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<AboutMeSkillData>? Middleware { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<AboutMeSkillData>? Sql { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<string>? Description { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AboutMeSkillData
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public int Proficiency { get; set; }
    }
}
