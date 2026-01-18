using GameArchiver.Models;
using GameArchiver.Services;
using System.Runtime.Versioning;
using System.Text;

namespace GameArchiver
{
    [SupportedOSPlatform("windows")]
    internal static class Program
    {
        private const string RequiredInstallPrefix = @"C:\GameArchiver\";

        static int Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            ConsoleUI.SetConsoleTheme();
            Console.Title = "GameArchiver";
            Console.Clear();
            ConsoleUI.WriteBanner();
            ConsoleUI.WriteLineGold("Initializing GameArchiver...\n");
            // REGISTRY
            RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\GameArchiver", "LastRun", "STRING", DateTime.Now.ToString("u"));
            // Generates a Policy folder for future use
            RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\Policies\GameArchiver", "Temp", "STRING", "GA");
            RegistryWorker.DeleteFromRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\Policies\GameArchiver", "Temp"); 

            try
            {
                // Resolve installer directory
                string installerDir = FileDiscoveryService.ResolveInstallerDir(args);

                // Find and load manifest
                var manifest = ManifestService.LoadManifest(installerDir);

                // Validate and sanitize InstallDir
                string installDir = ManifestService.ValidateInstallDir(manifest.InstallDir, RequiredInstallPrefix);

                // Update console title with short name and setup type
                string titleSuffix = manifest.SetupType ?? "Installer";
                Console.Title = $"GameArchiver - {manifest.ShortName} ({titleSuffix})";
                RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\GameArchiver", "LastManifestName", "STRING", manifest.GameName);
                // Locate required files
                var files = FileDiscoveryService.LocateFiles(installerDir, manifest);
                string? requirementsFolder = FileDiscoveryService.LocateRequirementsFolder(installerDir);
                string? tosFile = FileDiscoveryService.LocateTermsOfService(installerDir, manifest);

                // Display copyright/trademark section
                DisplayCopyrightSection(manifest);

                // Display manifest info section
                DisplayManifestInfoSection(manifest);

                // Display source section
                DisplaySourceSection(installerDir, files, manifest, requirementsFolder, tosFile);

                // Display target section
                ConsoleUI.WriteSection("TARGET");
                Console.WriteLine($"Install to: {installDir}");
                Console.WriteLine();

                // Handle Terms of Service
                if (manifest.AcceptTermsOfServiceRequired)
                {
                    if (!TermsOfServiceService.HandleTermsOfService(installerDir, manifest, tosFile))
                    {
                        RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\GameArchiver", "LastStatus", "STRING", "ToSDeclined");
                        ConsoleUI.WriteLineRed("You need to accept the Terms of Service to continue. Installation cancelled.");
                        Console.WriteLine();
                        Console.WriteLine("Press any key to exit...");
                        Console.ReadKey(intercept: true);
                        return 2;
                    }
                }

                if (!ConsoleUI.Confirm("Proceed with verification + install? (Y/N): "))
                    return 2;

                // Verify integrity
                ConsoleUI.WriteSection("VERIFYING INTEGRITY");
                RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\GameArchiver", "LastStatus", "STRING", "VERIFY");
                ConsoleUI.RunWithSpinner("Computing SHA-256...", () =>
                {
                    VerificationService.VerifySha256(files.ArchivePath, files.HashPath);
                });
                ConsoleUI.WriteLineGreen("Integrity OK.\n");

                // Install
                ConsoleUI.WriteSection("INSTALLING");
                RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\GameArchiver", "LastStatus", "STRING", "INSTALL");
                Directory.CreateDirectory(installDir);

                // Extract with progress (handle password if needed)
                ExtractionService.Extract7zWithProgress(
                    files.ArchivePath, 
                    installDir, 
                    manifest.PasswordProtectedArchive ? manifest.ArchivePassword : null);

                // Copy manual if exists
                string? manualDest = null;
                if (manifest.ManualExists && files.ManualPath != null)
                {
                    RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\GameArchiver", "LastStatus", "STRING", "ManualCopy");
                    manualDest = Path.Combine(installDir, Path.GetFileName(files.ManualPath));
                    File.Copy(files.ManualPath, manualDest, overwrite: true);
                }

                // Copy requirements folder if found
                if (!string.IsNullOrEmpty(requirementsFolder))
                {
                    RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\GameArchiver", "LastStatus", "STRING", "RequirementsCopy");
                    string requirementsDest = Path.Combine(installDir, "requirements");
                    ConsoleUI.RunWithSpinner("Copying requirements folder...", () =>
                    {
                        FileOperations.CopyDirectory(requirementsFolder, requirementsDest);
                    });
                }

                // Copy Terms of Service if exists
                if (tosFile != null)
                {
                    RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\GameArchiver", "LastStatus", "STRING", "ToSCopy");
                    string tosDest = Path.Combine(installDir, Path.GetFileName(tosFile));
                    File.Copy(tosFile, tosDest, overwrite: true);
                }

                ConsoleUI.WriteLineGreen("\nInstall complete.");

                // Execute custom steps
                if (manifest.CustomSteps != null && manifest.CustomSteps.Count > 0)
                {
                    ConsoleUI.WriteSection("CUSTOM STEPS");
                    RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\GameArchiver", "LastStatus", "STRING", "CustomSteps");
                    CustomStepsService.ExecuteCustomSteps(manifest.CustomSteps, installDir);
                }

                // Opening section
                ConsoleUI.WriteSection("OPENING");
                ConsoleUI.RunWithSpinner("Opening install folder...", () => FileOperations.OpenFolder(installDir));

                if (manualDest != null)
                {
                    ConsoleUI.RunWithSpinner("Opening manual...", () => FileOperations.OpenFile(manualDest));
                }

                // Open Terms of Service on launch if configured
                if (manifest.OpenToSOnLaunch && !string.IsNullOrWhiteSpace(manifest.TermsOfServiceWebSrc))
                {
                    ConsoleUI.RunWithSpinner("Opening Terms of Service...", () => FileOperations.OpenUrl(manifest.TermsOfServiceWebSrc));
                }

                // Launch main executable
                string mainExeFullPath = Path.Combine(installDir, manifest.MainExePath);
                if (File.Exists(mainExeFullPath))
                {
                    ConsoleUI.RunWithSpinner($"Opening {manifest.MainExeName}...", () => FileOperations.OpenFile(mainExeFullPath));
                }
                else
                {
                    ConsoleUI.WriteLineRed($"{manifest.MainExeName} not found at: {mainExeFullPath}");
                }
                RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\GameArchiver", "LastStatus", "STRING", "DONE");
                ConsoleUI.WriteLineGold("\nAll done.");
                Console.WriteLine();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey(intercept: true);
                Console.ResetColor();
                return 0;
            }
            catch (Exception ex)
            {
                RegistryWorker.WriteToRegistry(@"HKEY_CURRENT_USER\Software\elNino0916\GameArchiver", "LastStatus", "STRING", "FAIL");
                ConsoleUI.WriteLineRed("\nFAILED:");
                Console.ResetColor();
                Console.WriteLine(ex.Message);
                Console.WriteLine();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey(intercept: true);
                return 1;
            }
        }

        private static void DisplayCopyrightSection(InstallerManifest manifest)
        {
            ConsoleUI.WriteSection("COPYRIGHT / TRADEMARK");
            Console.WriteLine($"GameArchiver (c) 2026 elNino0916 and contributors. | {manifest.GameName} (c) {manifest.CopyrightYear} {manifest.Developer}");
            if (manifest.TrademarkExists)
            {
                Console.WriteLine($"Trademark: {manifest.TrademarkOwner}");
            }
            Console.WriteLine();
        }

        private static void DisplayManifestInfoSection(InstallerManifest manifest)
        {
            ConsoleUI.WriteSection("MANIFEST INFO");
            var info = new System.Collections.Generic.List<string>();
            
            if (!string.IsNullOrWhiteSpace(manifest.GAManifestVersion))
                info.Add($"Schema: {manifest.GAManifestVersion}");
            if (!string.IsNullOrWhiteSpace(manifest.GameVersion))
                info.Add($"Game: v{manifest.GameVersion}");
            if (!string.IsNullOrWhiteSpace(manifest.SetupType))
                info.Add($"Type: {manifest.SetupType}");
            if (manifest.PasswordProtectedArchive)
                info.Add("Password Protected");
            
            Console.WriteLine(string.Join(" | ", info));
            
            if (!string.IsNullOrWhiteSpace(manifest.SetupDesc))
                Console.WriteLine($"Desc: {manifest.SetupDesc}");
            
            Console.WriteLine();
        }

        private static void DisplaySourceSection(string installerDir, InstallerFiles files, 
            InstallerManifest manifest, string? requirementsFolder, string? tosFile)
        {
            ConsoleUI.WriteSection("SOURCE");
            Console.WriteLine($"Dir:     {installerDir}");
            Console.WriteLine($"Archive: {Path.GetFileName(files.ArchivePath)}");
            
            var extras = new System.Collections.Generic.List<string>();
            if (manifest.ManualExists && files.ManualPath != null)
                extras.Add($"Manual: {Path.GetFileName(files.ManualPath)}");
            if (tosFile != null)
                extras.Add($"ToS: {Path.GetFileName(tosFile)}");
            if (requirementsFolder != null)
                extras.Add($"Reqs: ✓");
            else
                extras.Add($"Reqs: ✗");
            
            if (extras.Count > 0)
                Console.WriteLine(string.Join(" | ", extras));
            
            Console.WriteLine();
        }
    }
}
 
