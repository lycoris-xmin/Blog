using Lycoris.Common.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Lycoris.Blog.Server.PropertyAttribute
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class StringValidAttribute : ValidationAttribute
    {
        private readonly string PrpertyName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PrpertyName"></param>
        public StringValidAttribute(string PrpertyName)
        {
            this.PrpertyName = PrpertyName;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Required { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public int MinLength { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public int MaxLength { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public string Regex { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object? value)
        {
            if (value == null)
            {
                ErrorMessage = $"{PrpertyName}不能为空";
                return !Required;
            }

            if (value is not string)
            {
                if (!Required)
                    return true;

                ErrorMessage = $"{PrpertyName}格式错误";
                return false;
            }

            var tmp = (string)value;

            if (MinLength > 0 && tmp.Length < MinLength)
            {
                ErrorMessage = $"{PrpertyName}长度不能小于{MinLength}字符";
                return false;
            }

            if (MaxLength > 0 && tmp.Length > MaxLength)
            {
                ErrorMessage = $"{PrpertyName}长度不能大于{MaxLength}字符";
                return false;
            }

            if (!Regex.IsNullOrEmpty() && new Regex(Regex!).IsMatch(tmp))
            {
                ErrorMessage = $"{PrpertyName}格式错误,正则验证失败";
                return false;
            }

            return true;
        }
    }
}
