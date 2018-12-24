using CSharpAnalyze.Domain.PublicInterfaces;
using System.Collections.Generic;

namespace CSharpAnalyze.Domain.Model.Analyze.Operations
{
  /// <summary>
  /// Operation系のスーパークラス
  /// </summary>
  internal abstract class AbstractOperation
  {
    /// <summary>
    /// Expression ValueObject
    /// </summary>
    public List<IExpression> Expressions { get; } = new List<IExpression>();
  }
}
