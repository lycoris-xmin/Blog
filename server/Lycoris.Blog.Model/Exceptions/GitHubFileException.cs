using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lycoris.Blog.Model.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class GitHubFileException : Exception
    {

        /// <summary>
        /// 
        /// </summary>
        public UploadFileResultEnum UploadFileResult { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UploadFileResult"></param>
        /// <param name="message"></param>
        public GitHubFileException(UploadFileResultEnum UploadFileResult, string message) : base(message)
        {
            this.UploadFileResult = UploadFileResult;
        }
    }
}
