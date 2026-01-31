using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace MultiRtspViewer.Services.AI
{
    public class ModelManager
    {
        private static readonly string ModelsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AIModels");

        public ModelManager()
        {
            if (!Directory.Exists(ModelsDirectory))
            {
                Directory.CreateDirectory(ModelsDirectory);
            }
        }

        public string GetModelPath(string modelFileName)
        {
            return Path.Combine(ModelsDirectory, modelFileName);
        }

        public bool IsModelAvailable(string modelFileName)
        {
            return File.Exists(GetModelPath(modelFileName));
        }

        public async Task EnsureModelExistsAsync(string modelFileName, string downloadUrl, IProgress<double>? progress = null)
        {
            var filePath = GetModelPath(modelFileName);
            if (File.Exists(filePath)) return;

            using var client = new HttpClient();
            using var response = await client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength ?? -1L;
            var canReportProgress = totalBytes != -1 && progress != null;

            using var contentStream = await response.Content.ReadAsStreamAsync();
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

            var totalRead = 0L;
            var buffer = new byte[8192];
            var isMoreToRead = true;

            while (isMoreToRead)
            {
                var read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                if (read == 0)
                {
                    isMoreToRead = false;
                }
                else
                {
                    await fileStream.WriteAsync(buffer, 0, read);

                    totalRead += read;
                    if (canReportProgress)
                    {
                        progress?.Report((double)totalRead / totalBytes * 100);
                    }
                }
            }
        }
    }
}
