namespace Lycoris.Blog.EntityFrameworkCore.Common.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TableColumnAttribute : Attribute
    {
        /// <summary>
        /// 主键
        /// </summary>
        public bool IsPrimary { get; set; } = false;

        /// <summary>
        /// 主键自增
        /// </summary>
        public bool IsIdentity { get; set; } = false;

        /// <summary>
        /// 乐观锁
        /// </summary>
        public bool IsRowVersion { get; set; } = false;

        /// <summary>
        /// 字符长度限制
        /// </summary>
        public int StringLength { get; set; } = 0;

        /// <summary>
        /// 默认值
        /// </summary>
        public object? DefaultValue { get; set; }

        /// <summary>
        /// 必填项
        /// </summary>
        public bool Required { get; set; } = true;

        /// <summary>
        /// 映射数据库类型
        /// </summary>
        public string? ColumnType { get; set; }

        /// <summary>
        /// Json列
        /// 实体必须含有无参构造函数
        /// </summary>
        public bool JsonMap { get; set; } = false;

        /// <summary>
        /// 敏感信息加密
        /// </summary>
        public bool Sensitive { get; set; } = false;

        /// <summary>
        /// 密码加密
        /// </summary>
        public bool SqlPassword { get; set; } = false;
    }
}
