﻿using CSharpAnalyze.Domain.Event;
using CSharpAnalyze.Domain.PublicInterfaces;
using CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems;
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
  internal class ItemStatementLocalDeclaration : AbstractItem, IItemStatementLocalDeclaration
  {
    /// <summary>
    /// 型リスト
    /// </summary>
    public List<IExpression> Types { get; } = new List<IExpression>();

    /// <summary>
    /// デフォルト設定リスト
    /// </summary>
    public List<IExpression> DefaultValues { get; } = new List<IExpression>();

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
    /// <param name="semanticModel">対象ソースのsemanticModel</param>
    /// <param name="parent">親IAnalyzeItem</param>
    /// <param name="container">イベントコンテナ</param>
    public ItemStatementLocalDeclaration(LocalDeclarationStatementSyntax node, SemanticModel semanticModel, IAnalyzeItem parent, EventContainer container) : base(parent, node, semanticModel, container)
    {
      ItemType = ItemTypes.MethodStatement;

      var declaredSymbol = semanticModel.GetDeclaredSymbol(node.Declaration.Variables.First());

      // 型推論
      IsVar = node.Declaration.Type.IsVar;

      // 型設定
      var parts = ((ILocalSymbol)declaredSymbol).Type.ToDisplayParts(SymbolDisplayFormat.MinimallyQualifiedFormat);
      foreach (var part in parts)
      {
        // スペースの場合は型設定に含めない
        if(part.Kind == SymbolDisplayPartKind.Space)
        {
          continue;
        }

        var name = Expression.GetSymbolName(part, true);
        var type = Expression.GetSymbolTypeName(part.Symbol);
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
      DefaultValues.AddRange(OperationFactory.GetExpressionList(initializer, container));
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
      }
      result.Append(";");

      return result.ToString();
    }

    #endregion
  }
}