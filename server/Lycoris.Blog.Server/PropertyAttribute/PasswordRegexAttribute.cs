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
    public class PasswordRegexAttribute : ValidationAttribute
    {
        private readonly string _passwordRegex;

        /// <summary>
        /// 
        /// </summary>
        public PasswordRegexAttribute() => _passwordRegex = PropertyRegex.Password;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="passwordRegex"></param>
        public PasswordRegexAttribute(string passwordRegex) => _passwordRegex = passwordRegex;


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
                return true;

            if (value is not string)
            {
                ErrorMessage = $"{ParamName}格式错误";
                return false;
            }

            var password = (string)value;
            if (password.IsNullOrEmpty())
                return true;

            var regex = new Regex(_passwordRegex);
            if (!regex.IsMatch(password))
            {
                ErrorMessage = $"{ParamName}格式错误";
                return false;
            }

            return true;
        }
    }
}
