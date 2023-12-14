using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lycoris.Blog.Application.AppServices.FileManage.Dtos
{
    public class UploadFileDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public FileTypeEnum FileType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long FileSize { get; set; }
    }
}
