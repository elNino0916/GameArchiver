using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace GameArchiver.Services
{
    /// <summary>
    /// Handles SHA-256 verification of archive files.
    /// </summary>
    public static class VerificationService
    {
        public static void VerifySha256(string archivePath, string shaFilePath)
        {
            string expected = ReadExpectedSha256(shaFilePath);
            string actual = ComputeFileSha256(archivePath);

            if (!actual.Equals(expected, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"SHA-256 mismatch!\nExpected: {expected}\nActual:   {actual}");
            }
        }

        private static string ReadExpectedSha256(string shaFilePath)
        {
            var lines = File.ReadAllLines(shaFilePath)
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToArray();

            if (lines.Length == 0)
                throw new InvalidOperationException("SHA256 file is empty.");

            var line = lines[0];

            var hex = new string(line.Where(c =>
                (c >= '0' && c <= '9') ||
                (c >= 'a' && c <= 'f') ||
                (c >= 'A' && c <= 'F')).ToArray());

            if (hex.Length >= 64)
                hex = hex[..64];

            if (hex.Length != 64)
                throw new InvalidOperationException(
                    "Could not parse a 64-char SHA-256 hash from .sha256 file.");

            return hex.ToLowerInvariant();
        }

        private static string ComputeFileSha256(string path)
        {
            using var sha = SHA256.Create();
            using var stream = File.OpenRead(path);
            byte[] hash = sha.ComputeHash(stream);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }
    }
}
