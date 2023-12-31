﻿using Lycoris.Blog.EntityFrameworkCore.Common.Attributes;
using Lycoris.Blog.EntityFrameworkCore.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lycoris.Blog.EntityFrameworkCore.Tables
{
    /// <summary>
    /// 浏览统计地图
    /// </summary>
    [Table("Browse_WorldMap")]
    [TableIndex("Country", true)]
    public class BrowseWorldMap : MySqlBaseEntity<int>
    {
        /// <summary>
        /// 国家名称
        /// </summary>
        [TableColumn(StringLength = 100)]
        public string Country { get; set; } = "";

        /// <summary>
        /// 流量统计数
        /// </summary>
        public int Count { get; set; }
    }
}
