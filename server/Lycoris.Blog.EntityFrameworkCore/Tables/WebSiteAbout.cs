using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.EntityFrameworkCore.Shared;
using Lycoris.Blog.EntityFrameworkCore.Tables.Enums;
using Lycoris.Common.Extensions;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 关于
    /// </summary>
    [Table("WebSite_About")]
    public class WebSiteAbout : MySqlBaseEntity<string>
    {
        /// <summary>
        /// 配置名称
        /// </summary>
        [TableColumn(StringLength = 100)]
        public string AboutName { get; set; } = string.Empty;

        /// <summary>
        /// 配置值
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// 保存格式
        /// </summary>
        [TableColumn(DefaultValue = ConfigurationValueTypeEnum.String)]
        public ConfigurationValueTypeEnum ValueType { get; set; } = ConfigurationValueTypeEnum.String;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override List<object> SeedData() => GetConfiguration(typeof(Constants.AppAbout).GetFields());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieids"></param>
        /// <returns></returns>
        private static List<object> GetConfiguration(FieldInfo[]? fieids)
        {
            var list = new List<object>();

            if (fieids != null && fieids.Length > 0)
            {
                foreach (var fieid in fieids)
                {
                    var attr = ((ConfigurationAttribute?)Attribute.GetCustomAttribute(fieid, typeof(ConfigurationAttribute)));
                    if (attr == null)
                        continue;

                    list.Add(new WebSiteAbout()
                    {
                        Id = (string?)fieid.GetRawConstantValue() ?? "",
                        AboutName = attr.Description,
                        ValueType = attr.ValueType,
                        Value = attr.DefaultObject != null ? Activator.CreateInstance(attr.DefaultObject).ToJson(new JsonSerializerSettings()
                        {
                            NullValueHandling = NullValueHandling.Include,
                            DateFormatString = "yyyy-MM-dd HH:mm:ss",
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        }) : attr.DefaultValue ?? "",
                    });
                }
            }

            return list;
        }
    }
}
