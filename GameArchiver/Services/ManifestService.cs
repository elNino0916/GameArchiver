using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using GameArchiver.Models;

namespace GameArchiver.Services
{
    /// <summary>
    /// Handles loading and validation of installer manifests.
    /// </summary>
    public static class ManifestService
    {
        public static InstallerManifest LoadManifest(string installerDir)
        {
            var manifestFiles = Directory.EnumerateFiles(installerDir, "*.ga.json").ToList();

            if (manifestFiles.Count == 0)
            {
                throw new FileNotFoundException(
                    "No manifest file found.\n" +
                    "Expected exactly one *.ga.json file in the installer directory.\n" +
                    $"Directory: {installerDir}");
            }

            if (manifestFiles.Count > 1)
            {
                var fileNames = string.Join("\n - ", manifestFiles.Select(Path.GetFileName));
                throw new InvalidOperationException(
                    $"Multiple manifest files found. Expected exactly one *.ga.json file.\n" +
                    $"Found:\n - {fileNames}");
            }

            string manifestPath = manifestFiles[0];
            ConsoleUI.WriteLineGold($"Loading manifest: {Path.GetFileName(manifestPath)}\n");

            string json = File.ReadAllText(manifestPath);

            var jsonOptions = new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };

            var manifest = JsonSerializer.Deserialize<InstallerManifest>(json, jsonOptions);

            if (manifest == null)
            {
                throw new InvalidOperationException("Failed to parse manifest JSON.");
            }

            ValidateManifestFields(manifest);

            return manifest;
        }

        public static void ValidateManifestFields(InstallerManifest manifest)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(manifest.GameName))
                errors.Add("GameName is required");
            if (string.IsNullOrWhiteSpace(manifest.ShortName))
                errors.Add("ShortName is required");
            if (string.IsNullOrWhiteSpace(manifest.Publisher))
                errors.Add("Publisher is required");
            if (string.IsNullOrWhiteSpace(manifest.Developer))
                errors.Add("Developer is required");
            if (string.IsNullOrWhiteSpace(manifest.CopyrightYear))
                errors.Add("CopyrightYear is required");
            if (string.IsNullOrWhiteSpace(manifest.InstallDir))
                errors.Add("InstallDir is required");
            if (string.IsNullOrWhiteSpace(manifest.MainExePath))
                errors.Add("MainExePath is required");
            if (string.IsNullOrWhiteSpace(manifest.MainExeName))
                errors.Add("MainExeName is required");
            if (manifest.TrademarkExists && string.IsNullOrWhiteSpace(manifest.TrademarkOwner))
                errors.Add("TrademarkOwner is required when TrademarkExists is true");

            if (errors.Count > 0)
            {
                throw new InvalidOperationException(
                    "Invalid manifest. Missing required fields:\n - " +
                    string.Join("\n - ", errors));
            }
        }

        public static string ValidateInstallDir(string installDir, string requiredPrefix)
        {
            string normalized;
            try
            {
                normalized = Path.GetFullPath(installDir);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Invalid InstallDir path: {ex.Message}");
            }

            if (!normalized.StartsWith(requiredPrefix, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"InstallDir must be under {requiredPrefix}\n" +
                    $"Provided: {normalized}");
            }

            string relativePart = normalized[requiredPrefix.Length..];
            if (relativePart.Contains(".."))
            {
                throw new InvalidOperationException(
                    "InstallDir contains invalid path traversal (..)");
            }

            return normalized;
        }
    }
}
