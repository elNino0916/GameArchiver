using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using GameArchiver.Models;

namespace GameArchiver.Services
{
    /// <summary>
    /// Handles execution of custom installation steps.
    /// </summary>
    public static class CustomStepsService
    {
        public static void ExecuteCustomSteps(List<CustomStep> steps, string installDir)
        {
            foreach (var step in steps)
            {
                string fullPath = Path.Combine(installDir, step.Path);
                string description = step.Description ?? step.Path;

                if (!File.Exists(fullPath))
                {
                    ConsoleUI.WriteLineRed($"Step skipped - file not found: {step.Path}");
                    continue;
                }

                switch (step.Type.ToLowerInvariant())
                {
                    case "runmsi":
                        ConsoleUI.RunWithSpinner($"Running MSI: {description}...", () =>
                        {
                            RunMsi(fullPath);
                        });
                        ConsoleUI.WriteLineGreen($"Completed: {description}");
                        break;

                    case "runexe":
                        ConsoleUI.RunWithSpinner($"Running: {description}...", () =>
                        {
                            RunExecutable(fullPath);
                        });
                        ConsoleUI.WriteLineGreen($"Completed: {description}");
                        break;

                    case "openfile":
                        ConsoleUI.RunWithSpinner($"Opening: {description}...", () =>
                        {
                            FileOperations.OpenFile(fullPath);
                        });
                        break;

                    case "openfolder":
                        string folderPath = Path.GetDirectoryName(fullPath) ?? installDir;
                        ConsoleUI.RunWithSpinner($"Opening folder: {description}...", () =>
                        {
                            FileOperations.OpenFolder(folderPath);
                        });
                        break;

                    default:
                        ConsoleUI.WriteLineRed($"Unknown step type: {step.Type}");
                        break;
                }
            }
        }

        private static void RunMsi(string msiPath)
        {
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "msiexec.exe",
                Arguments = $"/i \"{msiPath}\"",
                UseShellExecute = true,
                WorkingDirectory = Path.GetDirectoryName(msiPath)
            });

            process?.WaitForExit();
        }

        private static void RunExecutable(string exePath)
        {
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = exePath,
                UseShellExecute = true,
                WorkingDirectory = Path.GetDirectoryName(exePath)
            });

            process?.WaitForExit();
        }
    }
}
