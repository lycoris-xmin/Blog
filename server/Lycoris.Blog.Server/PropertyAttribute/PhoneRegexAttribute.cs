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
    public class PhoneRegexAttribute : ValidationAttribute
    {
        private readonly string _phoneRegex;

        /// <summary>
        /// 
        /// </summary>
        public PhoneRegexAttribute() => _phoneRegex = PropertyRegex.Phone;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phoneRegex"></param>
        public PhoneRegexAttribute(string phoneRegex) => _phoneRegex = phoneRegex;

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
                ErrorMessage = "手机号格式错误";
                return false;
            }

            var phone = (string)value;
            if (phone.IsNullOrEmpty())
                return true;

            var regex = new Regex(_phoneRegex);
            if (!regex.IsMatch(phone))
            {
                ErrorMessage = "手机号格式错误";
                return false;
            }

            return true;
        }
    }
}
