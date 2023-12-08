namespace Lycoris.Blog.Application.AppServices.SiteNavigations.Dtos
{
    public class SiteNavigationGroupDataDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int? GroupId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? GroupName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<SiteNavigationDataDto>? List { get; set; }
    }
}
