using CSharpAnalyze.Domain.Event;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis;

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
      var localType = (operation.DeclaredSymbol as ILocalSymbol).Type;
      var typeName = localType.Name;
      if (localType.SpecialType == SpecialType.None)
      {
        typeName = localType.TypeKind.ToString();
      }

      Expressions.Add(new Expression(operation.MatchedType.Name, typeName));
      Expressions.Add(new Expression(" ", ""));
      Expressions.Add(new Expression(operation.DeclaredSymbol.Name, typeName));
    }
  }
}
