using ICSharpCode.SharpZipLib.Zip;
using Lycoris.Autofac.Extensions;
using Lycoris.Blog.Common;
using Lycoris.Common.Extensions;

namespace Lycoris.Blog.Core.SharpFastZip.Impl
{
    [AutofacRegister(ServiceLifeTime.Transient)]
    public class SharpFastZipService : ISharpFastZipService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="zipFilePath"></param>
        /// <param name="sourcePath"></param>
        public void CreateZipFile(string zipFilePath, string sourcePath)
        {
            //
            var allFiles = GetAllFiles(sourcePath);

            var files = allFiles.GroupBy(x => x.Path).ToDictionary(x => x.Key, x => x);

            var fastZip = new FastZip() { CreateEmptyDirectories = true };
            fastZip.CreateZip(zipFilePath, AppSettings.Path.WebRootPath, true, "zip");

            using var zipFile = new ZipFile(zipFilePath);
            zipFile.BeginUpdate();

            foreach (var item in files)
            {
                foreach (var value in item.Value)
                {
                    zipFile.Add(Path.Combine(value.Path, value.FileName), Path.Combine(value.Path.Replace(AppSettings.Path.WebRootPath, ""), value.FileName));
                }
            }

            zipFile.CommitUpdate();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <returns></returns>
        private IReadOnlyList<FileTemp> GetAllFiles(string sourcePath)
        {
            var res = new List<FileTemp>();

            var files = Directory.GetFiles(sourcePath);

            if (files.Length > 0)
            {
                res.AddRange(files.Select(x => new FileTemp()
                {
                    FileName = Path.GetFileName(x),
                    Path = x.Replace(Path.GetFileName(x), "").TrimEnd('/').TrimEnd('\\')
                }));
            }

            var directories = Directory.GetDirectories(sourcePath);

            if (directories.Length > 0)
                directories.ForEach(x => res.AddRange(GetAllFiles(x)));

            return res;
        }


        private class FileTemp
        {
            /// <summary>
            /// 
            /// </summary>
            public string Path { get; set; } = string.Empty;

            /// <summary>
            /// 
            /// </summary>
            public string FileName { get; set; } = string.Empty;
        }
    }
}
