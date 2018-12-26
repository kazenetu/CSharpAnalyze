using CSharpAnalyze.Domain.PublicInterfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpAnalyze.Domain.Model.Analyze.Items
{
  /// <summary>
  /// アイテム：フィールド
  /// </summary>
  internal class ItemField : AbstractItem
  {
    /// <summary>
    /// フィールドの型リスト
    /// </summary>
    public List<IExpression> FieldTypes { get; } = new List<IExpression>();

    /// <summary>
    /// デフォルト設定リスト
    /// </summary>
    public List<IExpression> DefaultValues { get; } = new List<IExpression>();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="node">対象Node</param>
    /// <param name="target">対象ソースのsemanticModel</param>
    /// <param name="parent">親IAnalyzeItem</param>
    public ItemField(FieldDeclarationSyntax node, SemanticModel semanticModel, IAnalyzeItem parent) : base(parent,node,semanticModel)
    {
      ItemType = ItemTypes.Field;

      var declaredSymbol = semanticModel.GetDeclaredSymbol(node.Declaration.Variables.First());

      // フィールドの型設定
      var parts = ((IFieldSymbol)declaredSymbol).Type.ToDisplayParts(SymbolDisplayFormat.MinimallyQualifiedFormat);
      foreach (var part in parts)
      {
        var name = $"{part}";
        var type = GetSymbolTypeName(part.Symbol);
        if (part.Kind == SymbolDisplayPartKind.ClassName)
        {
          // 外部ファイル参照イベント発行
          RaiseOtherFileReferenced(node, part.Symbol);
        }

        FieldTypes.Add(new Expression(name, type));
      }

      // デフォルト設定
      var constantValue = node.Declaration.Variables.FirstOrDefault();
      if (constantValue?.Initializer == null)
      {
        return;
      }
      var initializer = semanticModel.GetOperation(constantValue.Initializer.Value);
      DefaultValues.AddRange(OperationFactory.GetExpressionList(initializer));
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

      // プロパティの型
      FieldTypes.ForEach(type => result.Append(type.Name));

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
