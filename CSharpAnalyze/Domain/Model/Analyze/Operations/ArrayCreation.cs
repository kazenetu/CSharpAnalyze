using CSharpAnalyze.Domain.Event;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace CSharpAnalyze.Domain.Model.Analyze.Operations
{
  /// <summary>
  /// Operation:ArrayCreation
  /// </summary>
  internal class ArrayCreation : AbstractOperation
  {
    /// <summary>
    /// NewキーワードのTypeName
    /// </summary>
    /// <remarks>配列の生成であることを格納する</remarks>
    private const string NewKeywordTypeName = "Array";

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="operation">IOperationインスタンス</param>
    public ArrayCreation(IArrayCreationOperation operation)
    {
      // newキーワード追加
      Expressions.Add(new Expression("new", NewKeywordTypeName));

      // 型
      var arrayTypeSymbol = operation.Type as IArrayTypeSymbol;
      var parts = arrayTypeSymbol?.ElementType.ToDisplayParts(SymbolDisplayFormat.MinimallyQualifiedFormat);
      foreach (var part in parts)
      {
        var name = $"{part}";
        var type = Expression.GetSymbolTypeName(part.Symbol);
        if (part.Kind == SymbolDisplayPartKind.ClassName)
        {
          // 外部ファイル参照イベント発行
          RaiseEvents.RaiseOtherFileReferenced(operation.Syntax, part.Symbol);
        }

        Expressions.Add(new Expression(name, type));
      }

      // 要素取得
      Expressions.Add(new Expression("[", string.Empty));
      for (var i = 0; i < operation.DimensionSizes.Length; i++)
      {
        Expressions.AddRange(OperationFactory.GetExpressionList(operation.DimensionSizes[i]));
        if (i >= 0 && i < operation.DimensionSizes.Length - 1)
        {
          Expressions.Add(new Expression(",", string.Empty));
        }
      }
      Expressions.Add(new Expression("]", string.Empty));
    }
  }
}
