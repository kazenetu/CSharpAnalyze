using CSharpAnalyze.Domain.Event;
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
    /// <param name="semanticModel">対象ソースのsemanticModel</param>
    /// <param name="container">イベントコンテナ</param>
    /// <param name="parent">親IAnalyzeItemインスタンス(デフォルトはnull)</param>
    /// <returns>IAnalyzeItemインスタンス</returns>
    public static IAnalyzeItem Create(SyntaxNode node, SemanticModel semanticModel, EventContainer container, IAnalyzeItem parent = null)
    {
      IAnalyzeItem result = null;

      // nodeの種類によって取得メソッドを実行
      switch (node)
      {
        // クラス定義
        case ClassDeclarationSyntax targetNode:
          result = new ItemClass(targetNode, semanticModel, parent, container);
          break;
        
        // クラス要素定義
        case PropertyDeclarationSyntax targetNode:
          result = new ItemProperty(targetNode, semanticModel, parent, container);
          break;
        case FieldDeclarationSyntax targetNode:
          result = new ItemField(targetNode, semanticModel, parent, container);
          break;
        case MethodDeclarationSyntax targetNode:
          result = new ItemMethod(targetNode, semanticModel, parent, container);
          break;
        case ConstructorDeclarationSyntax targetNode:
          result = new ItemConstructor(targetNode, semanticModel, parent, container);
          break;
        case EnumDeclarationSyntax targetNode:
          result = new ItemEnum(targetNode, semanticModel, parent, container);
          break;

        // ラムダ式
        case ArrowExpressionClauseSyntax targetNode:
          {
            var op = semanticModel.GetOperation(targetNode).Children.First();
            switch (op)
            {
              case Microsoft.CodeAnalysis.Operations.IReturnOperation operation:
                result =  new ItemReturn(op.Syntax, semanticModel, parent, container);
                break;
              case Microsoft.CodeAnalysis.Operations.IExpressionStatementOperation operation:
                result = new ItemStatementExpression(op.Syntax, semanticModel, parent, container);
                break;
            }
          }
          break;

        // ローカル定義
        case LocalFunctionStatementSyntax targetNode:
          result = new ItemLocalFunction(targetNode, semanticModel, parent, container);
          break;
        case LocalDeclarationStatementSyntax targetNode:
          result = new ItemStatementLocalDeclaration(targetNode, semanticModel, parent, container);
          break;
        case ExpressionStatementSyntax targetNode:
          result = new ItemStatementExpression(targetNode, semanticModel, parent, container);
          break;
        case AccessorDeclarationSyntax targetNode:
          result = new ItemAccessor(targetNode, semanticModel, parent, container);
          break;

        // 分岐処理
        case IfStatementSyntax targetNode:
          result = new ItemIf(targetNode, semanticModel, parent, container);
          break;
        case ElseClauseSyntax targetNode:
          result = new ItemElseClause(targetNode, semanticModel, parent, container);
          break;
        case SwitchStatementSyntax targetNode:
          result = new ItemSwitch(targetNode, semanticModel, parent, container);
          break;
        case SwitchSectionSyntax targetNode:
          result = new ItemSwitchCase(targetNode, semanticModel, parent, container);
          break;

        // ループ処理
        case WhileStatementSyntax targetNode:
          result = new ItemWhile(targetNode, semanticModel, parent, container);
          break;
        case ForEachStatementSyntax targetNode:
          result = new ItemForEach(targetNode, semanticModel, parent, container);
          break;
        case ForStatementSyntax targetNode:
          result = new ItemFor(targetNode, semanticModel, parent, container);
          break;

        // その他
        case ReturnStatementSyntax targetNode:
          result = new ItemReturn(targetNode, semanticModel, parent, container);
          break;
        case BreakStatementSyntax targetNode:
          result = new ItemBreak(targetNode, semanticModel, parent, container);
          break;
        case ContinueStatementSyntax targetNode:
          result = new ItemContinue(targetNode, semanticModel, parent, container);
          break;
      }

      return result;
    }
  }
}
