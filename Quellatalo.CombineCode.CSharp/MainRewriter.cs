using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Quellatalo.CombineCode.CSharp;

/// <summary>
/// A <see cref="CSharpSyntaxRewriter"/> that adds Solution class and Main method.
/// </summary>
/// <param name="mainClassName">The class name that contains Main.</param>
/// <param name="mainContent">The solver class name.</param>
public class MainRewriter(string mainClassName, string mainContent) : CSharpSyntaxRewriter
{
    readonly MethodDeclarationSyntax _main = SyntaxFactory
        .MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), "Main")
        .WithModifiers(
            SyntaxFactory.TokenList(
                SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                SyntaxFactory.Token(SyntaxKind.StaticKeyword)))
        .WithBody(
            SyntaxFactory.Block(
                CSharpSyntaxTree.ParseText(mainContent)
                    .GetRoot()
                    .DescendantNodes()
                    .OfType<ExpressionStatementSyntax>()
                    .First()));

    /// <inheritdoc/>
    public override SyntaxNode? VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
    {
        ArgumentNullException.ThrowIfNull(node);
        return node.DescendantNodes().OfType<ClassDeclarationSyntax>().Any(v => v.Identifier.Text == mainClassName)
            ? base.VisitNamespaceDeclaration(node)
            : node.WithMembers(
                    node.Members.Add(
                        SyntaxFactory.ClassDeclaration(mainClassName)
                            .WithModifiers([SyntaxFactory.Token(SyntaxKind.PublicKeyword)])
                            .WithMembers([_main])))
                .NormalizeWhitespace();
    }

    /// <inheritdoc/>
    public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        ArgumentNullException.ThrowIfNull(node);
        if (node.Identifier.Text == mainClassName &&
            node.DescendantNodes().OfType<MethodDeclarationSyntax>().All(syntax => syntax.Identifier.Text != "Main"))
        {
            return node.WithMembers(node.Members.Add(_main)).NormalizeWhitespace();
        }

        return base.VisitClassDeclaration(node);
    }
}
