using System;
using System.IO;
using System.Linq;
using GameArchiver.Models;

namespace GameArchiver.Services
{
    /// <summary>
    /// Handles file discovery for installer resources.
    /// </summary>
    public static class FileDiscoveryService
    {
        public static string ResolveInstallerDir(string[] args)
        {
            if (args.Length >= 1 && Directory.Exists(args[0]))
                return Path.GetFullPath(args[0]);

            var exeDir = AppContext.BaseDirectory;
            return Path.GetFullPath(exeDir);
        }

        public static InstallerFiles LocateFiles(string installerDir, InstallerManifest manifest)
        {
            string archiveBase = manifest.ArchiveNameOverride ?? manifest.GameName;

            // Find archive
            string preferredArchive = Path.Combine(installerDir, $"{archiveBase}.7z");
            string archive;
            bool archiveWarning = false;

            if (File.Exists(preferredArchive))
            {
                archive = preferredArchive;
            }
            else
            {
                archive = Directory.EnumerateFiles(installerDir, "*.7z").FirstOrDefault()
                    ?? throw new FileNotFoundException(
                        $"No .7z archive found in installer directory.\n" +
                        $"Expected: {archiveBase}.7z");
                archiveWarning = true;
            }

            if (archiveWarning)
            {
                ConsoleUI.WriteLineGold($"Warning: Using fallback archive: {Path.GetFileName(archive)}");
            }

            // Find hash file
            string preferredHash = archive + ".sha256";
            string hash;
            bool hashWarning = false;

            if (File.Exists(preferredHash))
            {
                hash = preferredHash;
            }
            else
            {
                hash = Directory.EnumerateFiles(installerDir, "*.sha256").FirstOrDefault()
                    ?? throw new FileNotFoundException(
                        "No .sha256 file found.\n" +
                        $"Expected: {Path.GetFileName(archive)}.sha256");
                hashWarning = true;
            }

            if (hashWarning)
            {
                ConsoleUI.WriteLineGold($"Warning: Using fallback hash file: {Path.GetFileName(hash)}");
            }

            // Find manual (only if ManualExists is true)
            string? manual = null;
            if (manifest.ManualExists)
            {
                if (!string.IsNullOrWhiteSpace(manifest.ManualPath))
                {
                    string specifiedManual = Path.Combine(installerDir, manifest.ManualPath);
                    if (File.Exists(specifiedManual))
                    {
                        manual = specifiedManual;
                    }
                }

                if (manual == null)
                {
                    manual = Directory.EnumerateFiles(installerDir, "*.pdf")
                        .OrderByDescending(p => Path.GetFileName(p)
                            .Contains("manual", StringComparison.OrdinalIgnoreCase))
                        .FirstOrDefault();
                }

                if (manual == null)
                {
                    ConsoleUI.WriteLineGold("Warning: ManualExists is true but no PDF found.");
                }
            }

            return new InstallerFiles(archive, hash, manual);
        }

        public static string? LocateRequirementsFolder(string installerDir)
        {
            return Directory.EnumerateDirectories(installerDir)
                .FirstOrDefault(d => Path.GetFileName(d)
                    .Equals("requirements", StringComparison.OrdinalIgnoreCase));
        }

        public static string? LocateTermsOfService(string installerDir, InstallerManifest manifest)
        {
            if (!manifest.AcceptTermsOfServiceRequired)
                return null;

            if (!string.IsNullOrWhiteSpace(manifest.TermsOfServiceSrc))
            {
                string specifiedTos = Path.Combine(installerDir, manifest.TermsOfServiceSrc);
                if (File.Exists(specifiedTos))
                {
                    return specifiedTos;
                }
            }

            return Directory.EnumerateFiles(installerDir, "*.txt")
                .FirstOrDefault(p =>
                {
                    string name = Path.GetFileName(p).ToLowerInvariant();
                    return name.Contains("tos") || name.Contains("terms") || name.Contains("eula") || name.Contains("license");
                });
        }
    }
}
