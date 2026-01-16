using System;
using System.IO;
using System.Linq;
using System.Threading;
using SharpCompress.Archives;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Common;

namespace GameArchiver.Services
{
    /// <summary>
    /// Handles 7z archive extraction with progress reporting.
    /// </summary>
    public static class ExtractionService
    {
        public static void Extract7zWithProgress(string archivePath, string destDir, string? password = null)
        {
            using var archive = !string.IsNullOrEmpty(password)
                ? SevenZipArchive.Open(archivePath, new SharpCompress.Readers.ReaderOptions { Password = password })
                : SevenZipArchive.Open(archivePath);

            var entries = archive.Entries
                .Where(e => !e.IsDirectory)
                .ToList();

            if (entries.Count == 0)
                throw new InvalidOperationException("Archive contains no files.");

            long totalBytes = entries.Where(e => e.Size > 0).Sum(e => e.Size);

            if (totalBytes <= 0)
            {
                ConsoleUI.RunWithSpinner("Extracting (size unknown)...", () =>
                {
                    archive.WriteToDirectory(destDir, new ExtractionOptions
                    {
                        ExtractFullPath = true,
                        Overwrite = true
                    });
                });
                return;
            }

            Console.WriteLine("Extracting:");
            ConsoleUI.DrawProgressBar(0);

            long done = 0;
            foreach (var entry in entries)
            {
                string outPath = Path.Combine(destDir,
                    entry.Key.Replace('/', Path.DirectorySeparatorChar));
                string? outDir = Path.GetDirectoryName(outPath);

                if (!string.IsNullOrEmpty(outDir))
                    Directory.CreateDirectory(outDir);

                using var entryStream = entry.OpenEntryStream();
                using var outStream = File.Create(outPath);

                CopyWithProgress(entryStream, outStream, bytesCopied =>
                {
                    long newDone = Interlocked.Add(ref done, bytesCopied);
                    int pct = (int)Math.Clamp((newDone * 100L) / totalBytes, 0, 100);
                    ConsoleUI.DrawProgressBar(pct);
                });
            }

            ConsoleUI.DrawProgressBar(100);
            Console.WriteLine();
        }

        private static void CopyWithProgress(Stream input, Stream output, Action<int> onChunk)
        {
            byte[] buffer = new byte[1024 * 256];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
                onChunk(read);
            }
        }
    }
}
