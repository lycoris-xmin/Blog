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
    public class PasswordValidAttribute : ValidationAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        public bool Required = false;

        /// <summary>
        /// 
        /// </summary>
        public string ParamName = "密码";

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

                ErrorMessage = $"{ParamName}格式错误";
                return false;
            }

            var password = (string)value;
            if (password.IsNullOrEmpty())
            {
                if (!Required)
                    return true;

                ErrorMessage = $"{ParamName}不能为空";
                return false;
            }

            var regex = new Regex(PropertyRegex.Password);
            if (!regex.IsMatch(password))
            {
                ErrorMessage = $"{ParamName}格式错误";
                return false;
            }

            return true;
        }
    }
}
