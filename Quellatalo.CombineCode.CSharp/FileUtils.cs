using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Quellatalo.CombineCode.CSharp;

/// <summary>
/// Utilities method related to solution files.
/// </summary>
public static class FileUtils
{
    static readonly string s_defaultExecutor = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "explorer" :
        RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "xdg-open" :
        RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "open" : string.Empty;

    static readonly EnumerationOptions s_enumerationOptions = new() { MatchCasing = MatchCasing.CaseSensitive };

    /// <summary>
    /// Scans the current directory and its ancestors to find the files that match the provided pattern.
    /// </summary>
    /// <param name="searchPattern">
    /// The search string to match against the names of files.
    /// This parameter can contain a combination of valid literal path and wildcard (* and ?) characters,
    /// but it doesn't support regular expressions.
    /// </param>
    /// <param name="startingDirectory">The starting directory to start the search.</param>
    /// <param name="subDirectories">Expected subdirectories to find in.</param>
    /// <returns>A list of strongly typed <see cref="FileInfo"/> objects.</returns>
    public static IReadOnlyList<FileInfo> FindFromParentDirectories(
        string searchPattern,
        string? startingDirectory = null,
        string[]? subDirectories = null)
    {
        List<FileInfo> searchResult = [];
        var directory = new DirectoryInfo(startingDirectory ?? Directory.GetCurrentDirectory());
        do
        {
            List<DirectoryInfo> candidates;
            if (subDirectories != null)
            {
                IEnumerable<DirectoryInfo> subCandidates = [directory];
                subCandidates = subDirectories.Aggregate(
                    subCandidates,
                    (current, subDirectory)
                        => current.SelectMany(info => info.GetDirectories(subDirectory, s_enumerationOptions)));
                candidates = subCandidates.ToList();
            }
            else
            {
                candidates = [directory];
            }

            searchResult.AddRange(candidates.SelectMany(info => info!.GetFiles(searchPattern, s_enumerationOptions)));
            directory = directory.Parent;
        }
        while (directory != null);

        return searchResult;
    }

    /// <summary>
    /// Opens the target file using the OS's default behavior.
    /// </summary>
    /// <param name="filePath">Target file to open.</param>
    public static void OpenFile(string filePath)
    {
        using var reportOpen = Process.Start(s_defaultExecutor, filePath);
    }
}
