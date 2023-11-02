namespace Lycoris.Blog.Core.Showdoc.Models
{
    public class ShowdocTemplate
    {
        public ShowdocMarkdownTemplateEnum Template { get; set; }

        public string? Title { get; set; }

        public string? Content { get; set; }


        internal string BuildMarndownContent()
        {
            return "";
        }
    }
}
