using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
//using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CapnpNet.Schema.Compiler
{
  public sealed class WhitespaceRewriter : CSharpSyntaxRewriter
  {
    public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
    {
      node = node.ReplaceNode(node.ParameterList, node.ParameterList.WithTrailingTrivia(SyntaxFactory.Space));
      return base.VisitConstructorDeclaration(node);
    }

    public override SyntaxNode VisitBlock(BlockSyntax node)
    {
      if (node.Statements.Count == 1
            /*&& (node.Parent is ConstructorDeclarationSyntax
              || node.Parent is AccessorDeclarationSyntax
              || node.Parent is MethodDeclarationSyntax)*/)
      {
        node = node.ReplaceToken(node.OpenBraceToken, node.OpenBraceToken.WithoutTrivia());
        node = node.ReplaceToken(node.CloseBraceToken, node.CloseBraceToken
          .WithLeadingTrivia()
          .WithTrailingTrivia(node.CloseBraceToken.TrailingTrivia.Last()));
        node = node.ReplaceNode(node.Statements[0], node.Statements[0]
          .WithLeadingTrivia(SyntaxFactory.Space)
          .WithTrailingTrivia(SyntaxFactory.Space));
      }

      return node;
    }

    public override SyntaxNode VisitAccessorDeclaration(AccessorDeclarationSyntax node)
    {
      node = node.ReplaceToken(node.Keyword, node.Keyword
        .WithLeadingTrivia(node.Keyword.LeadingTrivia.First())
        .WithTrailingTrivia(SyntaxFactory.Space));
      return base.VisitAccessorDeclaration(node);
    }
  }

}