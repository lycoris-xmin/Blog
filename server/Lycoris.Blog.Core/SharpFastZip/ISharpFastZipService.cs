namespace Lycoris.Blog.Core.SharpFastZip
{
    public interface ISharpFastZipService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="zipFilePath"></param>
        /// <param name="sourcePath"></param>
        void CreateZipFile(string zipFilePath, string sourcePath);
    }
}
