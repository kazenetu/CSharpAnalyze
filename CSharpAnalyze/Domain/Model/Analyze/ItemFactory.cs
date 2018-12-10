using CSharpAnalyze.Domain.Model.Analyze.Items;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpAnalyze.Domain.Model.Analyze
{
  /// <summary>
  /// IAnalyzeItemインスタンス作成クラス
  /// </summary>
  public static class ItemFactory
  {

    /// <summary>
    /// IAnalyzeItemインスタンス作成
    /// </summary>
    /// <param name="node">対象Node</param>
    /// <param name="target">対象ソースのsemanticModel</param>
    /// <returns>IAnalyzeItemインスタンス</returns>
    public static IAnalyzeItem Create(SyntaxNode node, SemanticModel semanticModel)
    {
      IAnalyzeItem result = null;

      // nodeの種類によって取得メソッドを実行
      switch (node)
      {
        case ClassDeclarationSyntax classDeclarationSyntax:
          result = new ItemClass(classDeclarationSyntax, semanticModel);
          break;
        case PropertyDeclarationSyntax propertyDeclarationSyntax:
          result = new ItemProperty(propertyDeclarationSyntax, semanticModel);
          break;
        case FieldDeclarationSyntax fieldDeclarationSyntax:
          result = new ItemField(fieldDeclarationSyntax, semanticModel);
          break;
        case MethodDeclarationSyntax methodDeclarationSyntax:
          result = new ItemMethod(methodDeclarationSyntax, semanticModel);
          break;
        case ConstructorConstraintSyntax constructorConstraintSyntax:
          // TODO コンストラクタクラス生成
          break;
      }

      return result;
    }
  }
}
