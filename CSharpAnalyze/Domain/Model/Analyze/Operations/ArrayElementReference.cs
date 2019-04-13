using CSharpAnalyze.Domain.Event;
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
    /// <param name="container">イベントコンテナ</param>
    public ArrayElementReference(IArrayElementReferenceOperation operation, EventContainer container) : base(container)
    {
      // 型名
      Expressions.AddRange(OperationFactory.GetExpressionList(operation.ArrayReference, container));

      // 要素取得
      Expressions.Add(new Expression("[", string.Empty));
      for (var i = 0; i < operation.Indices.Length; i++)
      {
        Expressions.AddRange(OperationFactory.GetExpressionList(operation.Indices[i], container));
        if (i >= 0 && i < operation.Indices.Length - 1)
        {
          Expressions.Add(new Expression(",", string.Empty));
        }
      }
      Expressions.Add(new Expression("]", string.Empty));
    }
  }
}
