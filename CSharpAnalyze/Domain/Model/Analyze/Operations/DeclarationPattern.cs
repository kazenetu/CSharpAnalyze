using CSharpAnalyze.Domain.Event;
using Microsoft.CodeAnalysis.Operations;

namespace CSharpAnalyze.Domain.Model.Analyze.Operations
{
  /// <summary>
  /// Operation:DeclarationPattern
  /// </summary>
  internal class DeclarationPattern : AbstractOperation
  {
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="operation">IOperationインスタンス</param>
    /// <param name="container">イベントコンテナ</param>
    public DeclarationPattern(IDeclarationPatternOperation operation, EventContainer container) : base(container)
    {
      Expressions.Add(new Expression(operation.MatchedType.Name, operation.MatchedType.TypeKind.ToString()));
      Expressions.Add(new Expression(" ", ""));
      Expressions.Add(new Expression(operation.DeclaredSymbol.Name, operation.DeclaredSymbol.Kind.ToString()));
    }
  }
}
