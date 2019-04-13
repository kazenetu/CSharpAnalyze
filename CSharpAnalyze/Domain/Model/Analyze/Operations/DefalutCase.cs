using CSharpAnalyze.Domain.Event;
using Microsoft.CodeAnalysis.Operations;

namespace CSharpAnalyze.Domain.Model.Analyze.Operations
{
  /// <summary>
  /// Operation:DefalutCase
  /// </summary>
  internal class DefalutCase : AbstractOperation
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="operation">IOperationインスタンス</param>
    /// <param name="container">イベントコンテナ</param>
    public DefalutCase(IDefaultCaseClauseOperation operation, EventContainer container) : base(container)
    {
      Expressions.Add(new Expression("default", operation.CaseKind.ToString()));
    }
  }
}
