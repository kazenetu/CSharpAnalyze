using CSharpAnalyze.Domain.Event;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using System.Text;

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
      // ローカルの型を取得
      var localType = (operation.DeclaredSymbol as ILocalSymbol).Type;

      // 型情報
      var typeName = localType.Name;
      if (localType.SpecialType == SpecialType.None)
      {
        typeName = localType.TypeKind.ToString();
      }

      // ローカルフィールドの型情報
      var localTypeName = GetLocalTypes(localType);

      // 型情報
      Expressions.Add(new Expression(localTypeName, typeName));

      Expressions.Add(new Expression(" ", ""));

      // ローカルフィールド
      Expressions.Add(new Expression(operation.DeclaredSymbol.Name, localTypeName));
    }

    /// <summary>
    /// ローカルの型を取得する
    /// </summary>
    /// <param name="target">ローカルのType</param>
    /// <returns>型情報</returns>
    private string GetLocalTypes(ITypeSymbol target)
    {
      var result = new StringBuilder();

      // 型設定
      var parts = target.ToDisplayParts(SymbolDisplayFormat.MinimallyQualifiedFormat);
      foreach (var part in parts)
      {
        // スペースの場合は型設定に含めない
        if (part.Kind == SymbolDisplayPartKind.Space)
        {
          continue;
        }

        var name = Expression.GetSymbolName(part, true);
        result.Append(name);
      }

      return result.ToString();
    }
  }
}
