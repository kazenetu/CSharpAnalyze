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
        case ClassDeclarationSyntax classDeclarationSyntax:
          result = new ItemClass(classDeclarationSyntax, semanticModel, parent);
          break;
        case PropertyDeclarationSyntax propertyDeclarationSyntax:
          result = new ItemProperty(propertyDeclarationSyntax, semanticModel, parent);
          break;
        case FieldDeclarationSyntax fieldDeclarationSyntax:
          result = new ItemField(fieldDeclarationSyntax, semanticModel, parent);
          break;
        case MethodDeclarationSyntax methodDeclarationSyntax:
          result = new ItemMethod(methodDeclarationSyntax, semanticModel, parent);
          break;
        case ConstructorConstraintSyntax constructorConstraintSyntax:
          // TODO コンストラクタクラス生成
          break;
        case LocalDeclarationStatementSyntax localDeclarationStatementSyntax:
          result = new ItemStatementLocalDeclaration(localDeclarationStatementSyntax, semanticModel, parent);
          break;
        case ExpressionStatementSyntax expressionStatementSyntax:
          result = new ItemStatementExpression(expressionStatementSyntax, semanticModel, parent);
          break;
      }

      return result;
    }
  }
}
