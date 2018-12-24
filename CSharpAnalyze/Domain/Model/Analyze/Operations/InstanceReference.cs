using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpAnalyze.Domain.Model.Analyze.Operations
{
  /// <summary>
  /// Operation:InstanceReference
  /// </summary>
  internal class InstanceReference : AbstractOperation
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="operation">IOperationインスタンス</param>
    public InstanceReference(IInstanceReferenceOperation operation)
    {
      Expressions.Add(new Expression($"{operation.Syntax}", operation.GetType().Name));
    }
  }
}
