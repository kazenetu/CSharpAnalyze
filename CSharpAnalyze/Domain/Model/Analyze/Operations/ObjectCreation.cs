using CSharpAnalyze.Domain.Event;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using System.Linq;

namespace CSharpAnalyze.Domain.Model.Analyze.Operations
{
  /// <summary>
  /// Operation:ObjectCreation
  /// </summary>
  internal class ObjectCreation : AbstractOperation
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="operation">IOperationインスタンス</param>
    public ObjectCreation(IObjectCreationOperation operation)
    {
      var node = operation.Syntax;
      var parts = operation.Type.ToDisplayParts(SymbolDisplayFormat.MinimallyQualifiedFormat);
      if (!parts.Any())
      {
        return;
      }

      // newキーワード追加
      Expressions.Add(new Expression("new", string.Empty));

      // クラス生成
      foreach (var part in parts)
      {
        var name = $"{part}";
        var type = Expression.GetSymbolTypeName(part.Symbol);
        if (part.Kind == SymbolDisplayPartKind.ClassName)
        {
          // 外部ファイル参照イベント発行
          RaiseEvents.RaiseOtherFileReferenced(node, part.Symbol);
        }

        Expressions.Add(new Expression(name, type));
      }

      // パラメーター取得
      Expressions.Add(new Expression("(", string.Empty));
      var isFirst = true;
      foreach(var param in operation.Arguments)
      {
        if (!isFirst)
        {
          Expressions.Add(new Expression(",", string.Empty));
        }
        Expressions.AddRange(OperationFactory.GetExpressionList(param));

        isFirst = false;
      }
      Expressions.Add(new Expression(")", string.Empty));
    }
  }
}
