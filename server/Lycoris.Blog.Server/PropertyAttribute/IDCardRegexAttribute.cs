using Lycoris.Blog.Server.Application.Constants;
using Lycoris.Common.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Lycoris.Blog.Server.PropertyAttribute
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IDCardRegexAttribute : ValidationAttribute
    {
        private readonly string _idCardRegex;

        /// <summary>
        /// 
        /// </summary>
        public IDCardRegexAttribute() => _idCardRegex = PropertyRegex.IDCard;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idCardRegex"></param>
        public IDCardRegexAttribute(string idCardRegex) => _idCardRegex = idCardRegex;

        /// <summary>
        /// 
        /// </summary>
        public string? Message = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object? value)
        {
            if (value == null)
                return true;

            if (value is not string)
            {
                ErrorMessage = Message ?? "身份证号格式错误";
                return false;
            }

            var idcard = (string)value;
            if (idcard.IsNullOrEmpty())
                return true;

            var regex = new Regex(_idCardRegex);
            if (!regex.IsMatch(idcard))
            {
                ErrorMessage = Message ?? "身份证号格式错误";
                return false;
            }

            return true;
        }

    }
}
