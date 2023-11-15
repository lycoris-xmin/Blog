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
    public class EmailRegexAttribute : ValidationAttribute
    {
        private readonly string _emailRegex;

        /// <summary>
        /// 
        /// </summary>
        public EmailRegexAttribute() => _emailRegex = PropertyRegex.Email;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="emailRegex"></param>
        public EmailRegexAttribute(string emailRegex) => _emailRegex = emailRegex;

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
                ErrorMessage = "邮箱格式错误";
                return false;
            }

            var email = (string)value;
            if (email.IsNullOrEmpty())
                return true;

            var regex = new Regex(_emailRegex);
            if (!regex.IsMatch(email))
            {
                ErrorMessage = "邮箱格式错误";
                return false;
            }

            return true;
        }
    }
}
