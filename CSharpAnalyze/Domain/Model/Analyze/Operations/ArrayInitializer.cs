using CSharpAnalyze.Domain.Event;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Generic;
using System.Linq;

namespace CSharpAnalyze.Domain.Model.Analyze.Operations
{
  /// <summary>
  /// Operation:ArrayInitializer
  /// </summary>
  internal class ArrayInitializer : AbstractOperation
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="operation">IOperationインスタンス</param>
    /// <param name="container">イベントコンテナ</param>
    public ArrayInitializer(IArrayInitializerOperation operation, EventContainer container) : base(container)
    {

      // 初期化要素
      Expressions.Add(new Expression("[", string.Empty));
      foreach (var element in operation.ElementValues)
      {
        Expressions.AddRange(OperationFactory.GetExpressionList(element, eventContainer));
        if (element != operation.ElementValues.Last())
        {
          Expressions.Add(new Expression(",", string.Empty));
        }
      }
      Expressions.Add(new Expression("]", string.Empty));
    }
  }
}
