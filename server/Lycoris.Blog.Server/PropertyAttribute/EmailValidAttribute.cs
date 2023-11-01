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
    public class EmailValidAttribute : ValidationAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Required = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object? value)
        {
            if (value == null)
                return !Required;

            if (value is not string)
            {
                if (!Required)
                    return true;

                ErrorMessage = "邮箱格式错误";
                return false;
            }

            var email = (string)value;
            if (email.IsNullOrEmpty())
            {
                if (!Required)
                    return true;

                ErrorMessage = "邮箱不能为空";
                return false;
            }

            var regex = new Regex(PropertyRegex.Email);
            if (!regex.IsMatch(email))
            {
                ErrorMessage = "邮箱格式错误";
                return false;
            }

            return true;
        }
    }
}
