using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GameArchiver.Models
{
    /// <summary>
    /// Represents the installer manifest loaded from a *.ga.json file.
    /// </summary>
    public class InstallerManifest
    {
        // Manifest schema (>=2.2.3)
        [JsonPropertyName("PasswordProtectedArchive")]
        public bool PasswordProtectedArchive { get; set; }

        [JsonPropertyName("ArchivePassword")]
        public string? ArchivePassword { get; set; }

        [JsonPropertyName("AcceptTermsOfServiceRequired")]
        public bool AcceptTermsOfServiceRequired { get; set; }

        [JsonPropertyName("TermsOfServiceWebSrc")]
        public string? TermsOfServiceWebSrc { get; set; }

        [JsonPropertyName("TermsOfServiceSrc")]
        public string? TermsOfServiceSrc { get; set; }

        [JsonPropertyName("OpenToSOnLaunch")]
        public bool OpenToSOnLaunch { get; set; }

        // Manifest schema (>=2.2.1)
        [JsonPropertyName("SetupType")]
        public string? SetupType { get; set; }

        [JsonPropertyName("SetupDesc")]
        public string? SetupDesc { get; set; }

        // Manifest schema (>=1.1)
        [JsonPropertyName("GAManifestVersion")]
        public string? GAManifestVersion { get; set; }

        [JsonPropertyName("GameVersion")]
        public string? GameVersion { get; set; }

        // Manifest schema (>=1.0)
        [JsonPropertyName("GameName")]
        public string GameName { get; set; } = "";

        [JsonPropertyName("ShortName")]
        public string ShortName { get; set; } = "";

        [JsonPropertyName("Publisher")]
        public string Publisher { get; set; } = "";

        [JsonPropertyName("DeveloperShortName")]
        public string DeveloperShortName { get; set; } = "";

        [JsonPropertyName("Developer")]
        public string Developer { get; set; } = "";

        [JsonPropertyName("CopyrightYear")]
        public string CopyrightYear { get; set; } = "";

        [JsonPropertyName("TrademarkOwner")]
        public string TrademarkOwner { get; set; } = "";

        [JsonPropertyName("TrademarkExists")]
        public bool TrademarkExists { get; set; }

        [JsonPropertyName("InstallDir")]
        public string InstallDir { get; set; } = "";

        [JsonPropertyName("MainExePath")]
        public string MainExePath { get; set; } = "";

        [JsonPropertyName("MainExeName")]
        public string MainExeName { get; set; } = "";

        [JsonPropertyName("ManualExists")]
        public bool ManualExists { get; set; }

        [JsonPropertyName("ManualPath")]
        public string? ManualPath { get; set; }

        [JsonPropertyName("ArchiveNameOverride")]
        public string? ArchiveNameOverride { get; set; }

        [JsonPropertyName("CustomSteps")]
        public List<CustomStep>? CustomSteps { get; set; }
    }
}
