using CSharpAnalyze.Domain.Model.Analyze.Items;
using CSharpAnalyze.Domain.PublicInterfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

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
        // クラス定義
        case ClassDeclarationSyntax targetNode:
          result = new ItemClass(targetNode, semanticModel, parent);
          break;
        
        // クラス要素定義
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
        case EnumDeclarationSyntax targetNode:
          result = new ItemEnum(targetNode, semanticModel, parent);
          break;

        // ラムダ式
        case ArrowExpressionClauseSyntax targetNode:
          {
            var op = semanticModel.GetOperation(targetNode).Children.First();
            switch (op)
            {
              case Microsoft.CodeAnalysis.Operations.IReturnOperation operation:
                result =  new ItemReturn(op.Syntax, semanticModel, parent);
                break;
              case Microsoft.CodeAnalysis.Operations.IExpressionStatementOperation operation:
                result = new ItemStatementExpression(op.Syntax, semanticModel, parent);
                break;
            }
          }
          break;

        // ローカル定義
        case LocalFunctionStatementSyntax targetNode:
          result = new ItemLocalFunction(targetNode, semanticModel, parent);
          break;
        case LocalDeclarationStatementSyntax targetNode:
          result = new ItemStatementLocalDeclaration(targetNode, semanticModel, parent);
          break;
        case ExpressionStatementSyntax targetNode:
          result = new ItemStatementExpression(targetNode, semanticModel, parent);
          break;
        case AccessorDeclarationSyntax targetNode:
          result = new ItemAccessor(targetNode, semanticModel, parent);
          break;

        // 分岐処理
        case IfStatementSyntax targetNode:
          result = new ItemIf(targetNode, semanticModel, parent);
          break;
        case ElseClauseSyntax targetNode:
          result = new ItemElseClause(targetNode, semanticModel, parent);
          break;
        case SwitchStatementSyntax targetNode:
          result = new ItemSwitch(targetNode, semanticModel, parent);
          break;
        case SwitchSectionSyntax targetNode:
          result = new ItemSwitchCase(targetNode, semanticModel, parent);
          break;

        // ループ処理
        case WhileStatementSyntax targetNode:
          result = new ItemWhile(targetNode, semanticModel, parent);
          break;
        case ForEachStatementSyntax targetNode:
          result = new ItemForEach(targetNode, semanticModel, parent);
          break;
        case ForStatementSyntax targetNode:
          result = new ItemFor(targetNode, semanticModel, parent);
          break;

        // その他
        case ReturnStatementSyntax targetNode:
          result = new ItemReturn(targetNode, semanticModel, parent);
          break;
        case BreakStatementSyntax targetNode:
          result = new ItemBreak(targetNode, semanticModel, parent);
          break;
        case ContinueStatementSyntax targetNode:
          result = new ItemContinue(targetNode, semanticModel, parent);
          break;
      }

      return result;
    }
  }
}
