using System;
using System.IO;
using GameArchiver.Models;

namespace GameArchiver.Services
{
    /// <summary>
    /// Handles Terms of Service acceptance workflow.
    /// </summary>
    public static class TermsOfServiceService
    {
        public static bool HandleTermsOfService(string installerDir, InstallerManifest manifest, string? tosFile)
        {
            bool hasOnlineToS = !string.IsNullOrWhiteSpace(manifest.TermsOfServiceWebSrc);
            bool hasLocalToS = tosFile != null;

            if (!hasOnlineToS && !hasLocalToS)
            {
                return true;
            }

            ConsoleUI.WriteSection("TERMS OF SERVICE");

            if (hasOnlineToS)
            {
                if (IsInternetAvailable())
                {
                    Console.WriteLine($"Opening: {manifest.TermsOfServiceWebSrc}");
                    FileOperations.OpenUrl(manifest.TermsOfServiceWebSrc!);
                }
                else if (hasLocalToS)
                {
                    Console.WriteLine($"No internet. Opening local: {Path.GetFileName(tosFile)}");
                    FileOperations.OpenFile(tosFile!);
                }
                else
                {
                    ConsoleUI.WriteLineGold("No ToS available (offline, no local file).");
                }
            }
            else if (hasLocalToS)
            {
                Console.WriteLine($"Opening: {Path.GetFileName(tosFile)}");
                FileOperations.OpenFile(tosFile!);
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Accept Terms of Service to continue? ");
            Console.ForegroundColor = ConsoleColor.Gray;
            return ConsoleUI.Confirm("(Y/N): ");
        }

        private static bool IsInternetAvailable()
        {
            try
            {
                using var client = new System.Net.Http.HttpClient();
                client.Timeout = TimeSpan.FromSeconds(3);
                using var response = client.GetAsync("https://www.google.com", System.Net.Http.HttpCompletionOption.ResponseHeadersRead).Result;
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
