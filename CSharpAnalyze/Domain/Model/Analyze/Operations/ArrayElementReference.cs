using Microsoft.CodeAnalysis.Operations;

namespace CSharpAnalyze.Domain.Model.Analyze.Operations
{
  /// <summary>
  /// Operation:ArrayElementReference 
  /// </summary>
  internal class ArrayElementReference : AbstractOperation
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="operation">IOperationインスタンス</param>
    public ArrayElementReference(IArrayElementReferenceOperation operation)
    {
      // 型名
      Expressions.AddRange(OperationFactory.GetExpressionList(operation.ArrayReference));

      // 要素取得
      Expressions.Add(new Expression("[", string.Empty));
      for (var i = 0; i < operation.Indices.Length; i++)
      {
        Expressions.AddRange(OperationFactory.GetExpressionList(operation.Indices[i]));
        if (i >= 0 && i < operation.Indices.Length - 1)
        {
          Expressions.Add(new Expression(",", string.Empty));
        }
      }
      Expressions.Add(new Expression("]", string.Empty));
    }
  }
}
