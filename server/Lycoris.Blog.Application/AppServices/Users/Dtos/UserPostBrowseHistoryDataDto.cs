using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lycoris.Blog.Application.AppServices.Users.Dtos
{
    public class UserPostBrowseHistoryDataDto
    {
        public long Id { get; set; }

        public long PostId { get; set; }

        public string? Icon { get; set; }

        public string? Title { get; set; }

        public string? Info { get; set; }

        public DateTime LastTime { get; set; }
    }
}
