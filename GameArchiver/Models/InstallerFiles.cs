namespace GameArchiver.Models
{
    /// <summary>
    /// Represents the located installer files.
    /// </summary>
    public record InstallerFiles(
        string ArchivePath,
        string HashPath,
        string? ManualPath
    );
}
