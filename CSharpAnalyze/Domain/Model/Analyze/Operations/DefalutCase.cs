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
    public DefalutCase(IDefaultCaseClauseOperation operation)
    {
      Expressions.Add(new Expression("default", operation.CaseKind.ToString()));
    }
  }
}
