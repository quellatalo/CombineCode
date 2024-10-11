using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Quellatalo.CombineCode.CSharp;

/// <summary>
/// A <see cref="CSharpSyntaxRewriter"/> that converts file-scoped namespace to block-scoped namespace.
/// </summary>
class CombinedCodeRewriter : CSharpSyntaxRewriter
{
    /// <inheritdoc/>
    public override SyntaxNode Visit(SyntaxNode? node)
    {
        ArgumentNullException.ThrowIfNull(node);
        if (node.SyntaxTree.GetRoot() == node)
        {
            var usingsOutsideNamespace = node.DescendantNodes()
                .OfType<UsingDirectiveSyntax>()
                .Where(u => u.Parent is CompilationUnitSyntax)
                .ToArray();
            if (usingsOutsideNamespace.Length != 0)
            {
                node = node.RemoveNodes(usingsOutsideNamespace, SyntaxRemoveOptions.KeepEndOfLine)!;
                var namespaceDeclaration = node.DescendantNodes().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
                if (namespaceDeclaration == null)
                {
                    var fileScopedNamespaceDeclaration = node.DescendantNodes()
                        .OfType<FileScopedNamespaceDeclarationSyntax>()
                        .FirstOrDefault();
                    if (fileScopedNamespaceDeclaration != null)
                    {
                        return node.ReplaceNode(
                            fileScopedNamespaceDeclaration,
                            VisitFileScopedNamespaceDeclaration(
                                fileScopedNamespaceDeclaration.AddUsings(usingsOutsideNamespace)));
                    }
                }
                else
                {
                    return node.ReplaceNode(
                        namespaceDeclaration,
                        namespaceDeclaration.AddUsings(usingsOutsideNamespace));
                }
            }
        }

        return base.Visit(node);
    }

    /// <inheritdoc />
    public override SyntaxNode VisitFileScopedNamespaceDeclaration(FileScopedNamespaceDeclarationSyntax node)
    {
        ArgumentNullException.ThrowIfNull(node);
        return SyntaxFactory
            .NamespaceDeclaration(
                node.AttributeLists,
                node.Modifiers,
                node.Name,
                node.Externs,
                node.Usings,
                node.Members)
            .WithLeadingTrivia(node.GetLeadingTrivia())
            .WithTrailingTrivia(node.GetTrailingTrivia())
            .NormalizeWhitespace();
    }
}
