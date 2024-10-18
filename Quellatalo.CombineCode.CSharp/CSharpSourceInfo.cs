using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Quellatalo.CombineCode.CSharp;

/// <summary>
/// Represents a CSharp source file.
/// </summary>
public class CSharpSourceInfo
{
    /// <summary>
    /// Gets the <see cref="DirectoryInfo"/> of the solution.
    /// </summary>
    public static readonly DirectoryInfo SolutionDirectory = FileUtils.FindFromParentDirectories("*.sln")[0].Directory!;

    static readonly CombinedCodeRewriter s_combinedCodeRewriter = new();
    static readonly Dictionary<string, CSharpSourceInfo> s_cachedSourceInfos = new();

    CSharpSourceInfo(string sourceFilePath)
    {
        SourceFileInfo = new FileInfo(sourceFilePath);
        SyntaxNode =
            s_combinedCodeRewriter.Visit(CSharpSyntaxTree.ParseText(File.ReadAllText(sourceFilePath)).GetRoot());
    }

    /// <summary>
    /// Gets the <see cref="FileInfo"/> of the source.
    /// </summary>
    public FileInfo SourceFileInfo { get; }

    /// <summary>
    /// Gets the <see cref="SyntaxNode"/>.
    /// </summary>
    public SyntaxNode? SyntaxNode { get; }

    /// <summary>
    /// Reads a <see cref="CSharpSourceInfo"/> from file.
    /// </summary>
    /// <param name="cSharpFilePath">Target CSharp file to read from.</param>
    /// <returns>A <see cref="CSharpSourceInfo"/> that represents the source.</returns>
    public static CSharpSourceInfo ReadFromFile(string cSharpFilePath)
    {
        if (s_cachedSourceInfos.TryGetValue(cSharpFilePath, out var sourceInfo))
        {
            return sourceInfo;
        }

        sourceInfo = new CSharpSourceInfo(cSharpFilePath);
        s_cachedSourceInfos[cSharpFilePath] = sourceInfo;
        return sourceInfo;
    }

    /// <summary>
    /// Reads <see cref="CSharpSourceInfo"/>s from multiple files.
    /// </summary>
    /// <param name="cSharpFiles">Target CSharp files to read from.</param>
    /// <returns>An array of <see cref="CSharpSourceInfo"/>.</returns>
    public static CSharpSourceInfo[] ReadFromFiles(IEnumerable<string> cSharpFiles)
        => [..cSharpFiles.Select(ReadFromFile)];

    /// <summary>
    /// Compile this CSharp source and its in-solution dependencies into one combined source.
    /// </summary>
    /// <param name="rewriter"><see cref="CSharpSyntaxRewriter"/> to modify the primary source.</param>
    /// <returns>A string of the combined source content.</returns>
    public string CompileToOneSource(CSharpSyntaxRewriter? rewriter = null)
    {
        var dependencies = InSolutionDependencies();
        var nodes = ReadFromFiles(dependencies.Except([SourceFileInfo.FullName])).Select(info => info.SyntaxNode);
        var builder = new StringBuilder();
        foreach (var node in nodes)
        {
            builder.AppendLine(node?.ToFullString());
        }

        return builder.AppendLine((rewriter is null ? SyntaxNode : rewriter.Visit(SyntaxNode))?.ToFullString())
            .ToString();
    }

    static void CollectDependencies(HashSet<string> dependencies, string dependency)
    {
        var directDependencies = ReadFromFile(dependency).InSolutionDirectDependencies();
        foreach (string directDependency in directDependencies)
        {
            if (dependencies.Add(directDependency))
            {
                CollectDependencies(dependencies, directDependency);
            }
        }
    }

    HashSet<string> InSolutionDependencies()
    {
        HashSet<string> dependencies = [];
        CollectDependencies(dependencies, SourceFileInfo.FullName);
        return dependencies;
    }

    IEnumerable<string> InSolutionDirectDependencies()
    {
        var usingDirectives = SyntaxNode?.DescendantNodes().OfType<UsingDirectiveSyntax>().ToArray() ?? [];
        foreach (var usingDirective in usingDirectives)
        {
            DirectoryInfo directoryInfo = new(
                Path.Combine([SolutionDirectory.FullName, ..usingDirective.NamespaceOrType.ToString().Split('.')]));
            if (!directoryInfo.Exists)
            {
                directoryInfo = new DirectoryInfo(
                    Path.Combine(
                        [SourceFileInfo.DirectoryName!, ..usingDirective.NamespaceOrType.ToString().Split('.')]));
                if (!directoryInfo.Exists)
                {
                    continue;
                }
            }

            foreach (var file in directoryInfo.GetFiles("*.cs"))
            {
                yield return file.FullName;
            }
        }

        var usedTypes = SyntaxNode?.DescendantNodes().OfType<TypeSyntax>().Select(d => d.ToString()).Distinct() ?? [];
        foreach (string usedType in usedTypes)
        {
            string[] dependencyPath = usedType.Split('.');
            string searchFile = $"{dependencyPath[^1]}.cs";
            string[]? subDirectories = null;
            if (dependencyPath.Length > 1)
            {
                subDirectories = dependencyPath[..^1];
            }
            else
            {
                if (searchFile == SourceFileInfo.Name)
                {
                    continue;
                }
            }

            string? found = FileUtils.FindFromParentDirectories(
                searchFile,
                SourceFileInfo.DirectoryName,
                subDirectories)?[0].FullName;
            if (found == null)
            {
                continue;
            }

            yield return found;
        }
    }
}
