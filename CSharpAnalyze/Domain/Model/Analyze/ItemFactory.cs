using CSharpAnalyze.Domain.Model.Analyze.Items;
using CSharpAnalyze.Domain.PublicInterfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpAnalyze.Domain.Model.Analyze
{
  /// <summary>
  /// IAnalyzeItemインスタンス作成クラス
  /// </summary>
  internal static class ItemFactory
  {

    /// <summary>
    /// IAnalyzeItemインスタンス作成
    /// </summary>
    /// <param name="node">対象Node</param>
    /// <param name="target">対象ソースのsemanticModel</param>
    /// <param name="parent">親IAnalyzeItemインスタンス(デフォルトはnull)</param>
    /// <returns>IAnalyzeItemインスタンス</returns>
    public static IAnalyzeItem Create(SyntaxNode node, SemanticModel semanticModel, IAnalyzeItem parent = null)
    {
      IAnalyzeItem result = null;

      // nodeの種類によって取得メソッドを実行
      switch (node)
      {
        case ClassDeclarationSyntax targetNode:
          result = new ItemClass(targetNode, semanticModel, parent);
          break;
        case PropertyDeclarationSyntax targetNode:
          result = new ItemProperty(targetNode, semanticModel, parent);
          break;
        case FieldDeclarationSyntax targetNode:
          result = new ItemField(targetNode, semanticModel, parent);
          break;
        case MethodDeclarationSyntax targetNode:
          result = new ItemMethod(targetNode, semanticModel, parent);
          break;
        case ConstructorDeclarationSyntax targetNode:
          result = new ItemConstructor(targetNode, semanticModel, parent);
          break;
        case LocalDeclarationStatementSyntax targetNode:
          result = new ItemStatementLocalDeclaration(targetNode, semanticModel, parent);
          break;
        case ExpressionStatementSyntax targetNode:
          result = new ItemStatementExpression(targetNode, semanticModel, parent);
          break;
        case IfStatementSyntax targetNode:
          result = new ItemIf(targetNode, semanticModel, parent);
          break;
        case ElseClauseSyntax targetNode:
          result = new ItemElseClause(targetNode, semanticModel, parent);
          break;
        case LocalFunctionStatementSyntax targetNode:
          result = new ItemLocalFunction(targetNode, semanticModel, parent);
          break;
        case WhileStatementSyntax targetNode:
          result = new ItemWhile(targetNode, semanticModel, parent);
          break;
      }

      return result;
    }
  }
}
