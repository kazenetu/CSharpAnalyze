﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpAnalyze.Domain.Model.Analyze.Items
{
  /// <summary>
  /// アイテム：プロパティ
  /// </summary>
  internal class ItemProperty : AbstractItem
  {
    /// <summary>
    /// プロパティの型リスト
    /// </summary>
    public List<Expression> PropertyTypes { get; } = new List<Expression>();

    /// <summary>
    /// デフォルト設定リスト
    /// </summary>
    public List<Expression> DefaultValues { get; } = new List<Expression>();

    /// <summary>
    /// アクセサリスト
    /// </summary>
    public List<string> AccessorList { get; } = new List<string>();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="node">対象Node</param>
    /// <param name="target">対象ソースのsemanticModel</param>
    /// <param name="parent">親IAnalyzeItem</param>
    public ItemProperty(PropertyDeclarationSyntax node, SemanticModel semanticModel, IAnalyzeItem parent) : base(parent, node, semanticModel)
    {
      ItemType = ItemTypes.Property;

      var declaredSymbol = semanticModel.GetDeclaredSymbol(node);

      // プロパティの型設定
      var parts = ((IPropertySymbol)declaredSymbol).Type.ToDisplayParts(SymbolDisplayFormat.MinimallyQualifiedFormat);
      foreach(var part in parts)
      {
        var name = $"{part}";
        var type = GetSymbolTypeName(part.Symbol);
        if (part.Kind == SymbolDisplayPartKind.ClassName)
        {
          // 外部ファイル参照イベント発行
          RaiseOtherFileReferenced(node,part.Symbol);
        }

        PropertyTypes.Add(new Expression(name, type));
      }

      // アクセサ設定
      AccessorList.AddRange(node.AccessorList.Accessors.Select(accessor => $"{accessor.Keyword}"));

      // デフォルト設定
      if (node.Initializer == null)
      {
        return;
      }
      var propertyInitializer = semanticModel.GetOperation(node.Initializer.Value);
      if (propertyInitializer.ConstantValue.HasValue)
      {
        // 値型
        var targetValue = propertyInitializer.ConstantValue;
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
        var tokens = propertyInitializer.Syntax.DescendantTokens();
        foreach(var token in tokens)
        {
          var symbol = semanticModel.GetSymbolInfo(token.Parent);
          DefaultValues.Add(new Expression(token.Value.ToString(), GetSymbolTypeName(symbol.Symbol)));

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

      // プロパティの型
      PropertyTypes.ForEach(type => result.Append(type.Name));

      // プロパティ名
      result.Append($" {Name}");

      // アクセサ
      result.Append(" {");
      AccessorList.ForEach(accessor => result.Append($" {accessor};"));
      result.Append(" }");

      if (DefaultValues.Any())
      {
        // デフォルト値
        result.Append(" = ");

        foreach(var value in DefaultValues)
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
