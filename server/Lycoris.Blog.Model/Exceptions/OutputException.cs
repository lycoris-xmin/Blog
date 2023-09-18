using Lycoris.Blog.Model.Global.Output;

namespace Lycoris.Blog.Model.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class OutputException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public ResCodeEnum ResCode { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string ResMsg { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ResCode"></param>
        public OutputException(ResCodeEnum ResCode) : base(ChangeMessage(ResCode))
        {
            this.ResCode = ResCode;
            this.ResMsg = "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ResCode"></param>
        /// <param name="ResMsg"></param>
        /// <param name="message"></param>
        public OutputException(ResCodeEnum ResCode, string? ResMsg = null, string? message = null) : base(ChangeMessage(ResCode, message ?? ResMsg))
        {
            this.ResCode = ResCode;
            this.ResMsg = ResMsg ?? "";
        }


        private static string ChangeMessage(ResCodeEnum resCode, string? resMsg = null)
        {
            return resCode switch
            {
                ResCodeEnum.Friendly => resMsg ?? "",
                ResCodeEnum.TokenExpired => "token is expired",
                _ => resMsg ?? "",
            };
        }
    }
}
