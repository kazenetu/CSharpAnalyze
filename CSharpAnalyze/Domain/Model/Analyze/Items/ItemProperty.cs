﻿using CSharpAnalyze.Domain.Event;
using CSharpAnalyze.Domain.PublicInterfaces;
using CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpAnalyze.Domain.Model.Analyze.Items
{
  /// <summary>
  /// アイテム：プロパティ
  /// </summary>
  internal class ItemProperty : AbstractItem, IItemProperty
  {
    /// <summary>
    /// プロパティの型リスト
    /// </summary>
    public List<IExpression> PropertyTypes { get; } = new List<IExpression>();

    /// <summary>
    /// デフォルト設定リスト
    /// </summary>
    public List<IExpression> DefaultValues { get; } = new List<IExpression>();

    /// <summary>
    /// アクセサリスト
    /// </summary>
    public List<IAnalyzeItem> AccessorList { get; } = new List<IAnalyzeItem>();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="node">対象Node</param>
    /// <param name="semanticModel">対象ソースのsemanticModel</param>
    /// <param name="parent">親IAnalyzeItem</param>
    /// <param name="container">イベントコンテナ</param>
    public ItemProperty(PropertyDeclarationSyntax node, SemanticModel semanticModel, IAnalyzeItem parent, EventContainer container) : base(parent, node, semanticModel, container)
    {
      ItemType = ItemTypes.Property;

      var declaredSymbol = semanticModel.GetDeclaredSymbol(node);

      // プロパティの型設定
      var parts = ((IPropertySymbol)declaredSymbol).Type.ToDisplayParts(SymbolDisplayFormat.MinimallyQualifiedFormat);
      foreach(var part in parts)
      {
        // スペースの場合は型設定に含めない
        if (part.Kind == SymbolDisplayPartKind.Space)
        {
          continue;
        }

        var name = Expression.GetSymbolName(part, true);
        var type = Expression.GetSymbolTypeName(part.Symbol);
        if (part.Kind == SymbolDisplayPartKind.ClassName)
        {
          // 外部ファイル参照イベント発行
          RaiseOtherFileReferenced(node,part.Symbol);
        }

        PropertyTypes.Add(new Expression(name, type));
      }

      // アクセサ設定
      if(node.AccessorList is null)
      {
        AccessorList.Add(ItemFactory.Create(node.ExpressionBody, semanticModel, container, this));
      }
      else
      {
        AccessorList.AddRange(node.AccessorList.Accessors.Select(accessor => ItemFactory.Create(accessor, semanticModel, container, this)));
      }

      // デフォルト設定
      if (node.Initializer == null)
      {
        return;
      }
      var propertyInitializer = semanticModel.GetOperation(node.Initializer.Value);
      DefaultValues.AddRange(OperationFactory.GetExpressionList(propertyInitializer, container));
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
      AccessorList.ForEach(accessor => result.Append($" {accessor.ToString(index + 1)}"));
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
