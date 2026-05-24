using System;
using System.IO;
using System.Linq;
using SevenZip;

namespace GameArchiver.Services
{
    /// <summary>
    /// Handles 7z archive extraction with progress reporting.
    /// </summary>
    public static class ExtractionService
    {
        /// <summary>
        /// Call once at startup, before any extraction.
        /// Provide the path to 7z.dll (Windows) or 7z.so (Linux).
        /// </summary>
        public static void SetLibraryPath(string libraryPath)
            => SevenZipBase.SetLibraryPath(libraryPath);

        public static void Extract7zWithProgress(string archivePath, string destDir, string? password = null)
        {
            SetLibraryPath("7za.dll");
            using var extractor = string.IsNullOrEmpty(password)
                ? new SevenZipExtractor(archivePath)
                : new SevenZipExtractor(archivePath, password);

            var fileEntries = extractor.ArchiveFileData
                .Where(e => !e.IsDirectory)
                .ToList();

            if (fileEntries.Count == 0)
                throw new InvalidOperationException("Archive contains no files.");

            // SevenZipSharp requires the output directory to exist.
            Directory.CreateDirectory(destDir);

            bool sizeKnown = fileEntries.Any(e => e.Size > 0);

            if (!sizeKnown)
            {
                ConsoleUI.RunWithSpinner("Extracting (size unknown)...", () =>
                    extractor.ExtractArchive(destDir));
                return;
            }

            Console.WriteLine("Extracting:");
            ConsoleUI.DrawProgressBar(0);

            // Fires periodically with the overall extraction percentage (0–100).
            extractor.Extracting += (_, e) =>
                ConsoleUI.DrawProgressBar(e.PercentDone);

            extractor.ExtractArchive(destDir);

            ConsoleUI.DrawProgressBar(100);
            Console.WriteLine();
        }
    }
}