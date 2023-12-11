using Lycoris.Blog.Application.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lycoris.Blog.Application.AppServices.Users.Dtos
{
    public class GetUserPostBrowseHistoryFilter : PageFilter
    {
        public DateTime? BeginTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
