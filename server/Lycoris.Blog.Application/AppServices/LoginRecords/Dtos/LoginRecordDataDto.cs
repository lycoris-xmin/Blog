namespace Lycoris.Blog.Application.AppServices.LoginRecords.Dtos
{
    public class LoginRecordDataDto
    {
        public string? UserAgent { get; set; }

        public uint Ip { get; set; }

        public string? IpAddress { get; set; }

        public DateTime LoginTime { get; set; }

        public bool Success { get; set; }

        public string? Remark { get; set; }
    }
}
