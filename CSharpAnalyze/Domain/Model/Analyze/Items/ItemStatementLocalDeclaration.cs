using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpAnalyze.Domain.Model.Analyze.Items
{
  /// <summary>
  /// アイテム：ローカル変数
  /// </summary>
  internal class ItemStatementLocalDeclaration : AbstractItem
  {
    /// <summary>
    /// 型リスト
    /// </summary>
    public List<Expression> Types { get; } = new List<Expression>();

    /// <summary>
    /// デフォルト設定リスト
    /// </summary>
    public List<Expression> DefaultValues { get; } = new List<Expression>();

    /// <summary>
    /// 型推論か否か
    /// </summary>
    public bool IsVar { get; }

    /// <summary>
    /// リテラルトークンリスト
    /// </summary>
    private readonly List<SyntaxKind> LiteralTokens = new List<SyntaxKind>() { SyntaxKind.StringLiteralToken, SyntaxKind.NumericLiteralToken, SyntaxKind.CharacterLiteralToken };

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="node">対象Node</param>
    /// <param name="target">対象ソースのsemanticModel</param>
    /// <param name="parent">親IAnalyzeItem</param>
    public ItemStatementLocalDeclaration(LocalDeclarationStatementSyntax node, SemanticModel semanticModel, IAnalyzeItem parent) : base(parent, node, semanticModel)
    {
      ItemType = ItemTypes.MethodStatement;

      var declaredSymbol = semanticModel.GetDeclaredSymbol(node.Declaration.Variables.First());

      // 型推論
      IsVar = node.Declaration.Type.IsVar;

      // 型設定
      var parts = ((ILocalSymbol)declaredSymbol).Type.ToDisplayParts(SymbolDisplayFormat.MinimallyQualifiedFormat);
      foreach (var part in parts)
      {
        var name = $"{part}";
        var type = GetSymbolTypeName(part.Symbol);
        if (part.Kind == SymbolDisplayPartKind.ClassName)
        {
          // 外部ファイル参照イベント発行
          RaiseOtherFileReferenced(node, part.Symbol);
        }

        Types.Add(new Expression(name, type));
      }

      // デフォルト設定
      var constantValue = node.Declaration.Variables.FirstOrDefault();
      if (constantValue?.Initializer == null)
      {
        return;
      }
      var initializer = semanticModel.GetOperation(constantValue.Initializer.Value);
      if (initializer.ConstantValue.HasValue)
      {
        // 値型
        var targetValue = initializer.ConstantValue;
        var name = targetValue.Value.ToString();
        if (targetValue.Value is string)
        {
          name = $"\"{name}\"";
        }
        DefaultValues.Add(new Expression(name, targetValue.Value.GetType().Name));
      }
      else
      {
        // クラスインスタンスなど
        var tokens = initializer.Syntax.DescendantTokens();
        foreach (var token in tokens)
        {
          var symbol = semanticModel.GetSymbolInfo(token.Parent);
          var targetValue = GetSymbolTypeName(symbol.Symbol);
          var name = token.Value.ToString();
          if (LiteralTokens.Contains(token.Kind()))
          {
            if(token.Value is string)
            {
              name = $"\"{name}\"";
            }
            targetValue = token.Value.GetType().Name;
          }

          DefaultValues.Add(new Expression(name, targetValue));

          if (symbol.Symbol != null && symbol.Symbol is INamedTypeSymbol)
          {
            // 外部ファイル参照イベント発行
            RaiseOtherFileReferenced(node, symbol.Symbol);
          }
        }
      }

    }

    #region 基本インターフェース実装：メソッド

    /// <summary>
    /// 文字列取得
    /// </summary>
    /// <param name="index">前スペース数</param>
    /// <returns>文字列</returns>
    public override string ToString(int index = 0)
    {
      var result = new StringBuilder();
      var indexSpace = string.Concat(Enumerable.Repeat("  ", index));

      foreach (var comment in Comments)
      {
        result.Append(indexSpace);
        result.AppendLine($"{comment}");
      }

      foreach (var modifier in Modifiers)
      {
        result.Append(indexSpace);
        result.Append($"{modifier} ");
      }
      result.Append(indexSpace);

      // プロパティの型
      if (IsVar)
      {
        result.Append("var");
      }
      else
      {
        Types.ForEach(type => result.Append(type.Name));
      }

      // プロパティ名
      result.Append($" {Name}");

      if (DefaultValues.Any())
      {
        // デフォルト値
        result.Append(" = ");

        foreach (var value in DefaultValues)
        {
          result.Append($"{value.Name}");
          if (value.Name == "new")
          {
            result.Append(" ");
          }
        }
        result.Append(";");
      }

      return result.ToString();
    }

    #endregion
  }
}