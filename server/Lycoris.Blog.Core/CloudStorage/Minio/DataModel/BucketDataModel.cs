namespace Lycoris.Blog.Core.CloudStorage.Minio.DataModel
{
    public class BucketDataModel
    {
        public string BucketName { get; set; } = string.Empty;

        public DateTime CreateTime { get; set; }
    }
}
