﻿using Lycoris.Blog.Model.Configurations;
using Minio;

namespace Lycoris.Blog.Core.Minio
{
    public interface IMinioFactory
    {
        MinioClient CreateClient(MinioConfiguration configuration);
    }
}
