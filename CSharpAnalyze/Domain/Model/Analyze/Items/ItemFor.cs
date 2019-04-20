using CSharpAnalyze.Domain.Event;
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
  /// アイテム：for
  /// </summary>
  internal class ItemFor : AbstractItem, IItemFor
  {
    /// <summary>
    /// 宣言部
    /// </summary>
    public List<List<IExpression>> Declarations { get; } = new List<List<IExpression>>();

    /// <summary>
    /// 宣言部が型推論か否か
    /// </summary>
    /// <remarks>定義済の場合は設定なし</remarks>
    public bool IsVar { get; }

    /// <summary>
    /// 宣言部の型リスト
    /// </summary>
    /// <remarks>定義済の場合は設定なし</remarks>
    public List<IExpression> Types { get; } = new List<IExpression>();

    /// <summary>
    /// 条件部
    /// </summary>
    public List<IExpression> Conditions { get; } = new List<IExpression>();

    /// <summary>
    /// 計算部
    /// </summary>
    public List<List<IExpression>> Incrementors { get; } = new List<List<IExpression>>();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="node">対象Node</param>
    /// <param name="semanticModel">対象ソースのsemanticModel</param>
    /// <param name="parent">親IAnalyzeItem</param>
    /// <param name="container">イベントコンテナ</param>
    public ItemFor(ForStatementSyntax node, SemanticModel semanticModel, IAnalyzeItem parent, EventContainer container) : base(parent, node, semanticModel, container)
    {
      ItemType = ItemTypes.MethodStatement;

      // 宣言部
      if(node.Declaration != null)
      {
        // 型設定
        IsVar = node.Declaration.Type.IsVar;
        var declaredSymbol = semanticModel.GetDeclaredSymbol(node.Declaration.Variables.First());
        var parts = ((ILocalSymbol)declaredSymbol).Type.ToDisplayParts(SymbolDisplayFormat.MinimallyQualifiedFormat);
        foreach (var part in parts)
        {
          // スペースの場合は型設定に含めない
          if (part.Kind == SymbolDisplayPartKind.Space)
          {
            continue;
          }

          var name = $"{part}";
          var type = Expression.GetSymbolTypeName(part.Symbol);
          if (part.Kind == SymbolDisplayPartKind.ClassName)
          {
            // 外部ファイル参照イベント発行
            RaiseOtherFileReferenced(node, part.Symbol);
          }

          Types.Add(new Expression(name, type));
        }

        // ローカル定義
        foreach (var variable in node.Declaration.Variables)
        {
          var declaration = semanticModel.GetOperation(variable);
          Declarations.Add(OperationFactory.GetExpressionList(declaration, container));
        }
      }
      else if (node.Initializers != null)
      {
        // 既存定義
        foreach (var initializer in node.Initializers)
        {
          var declaration = semanticModel.GetOperation(initializer);
          Declarations.Add(OperationFactory.GetExpressionList(declaration, container));
        }
      }

      // 計算部
      foreach (var increment in node.Incrementors)
      {
        var incrementOperator = semanticModel.GetOperation(increment);
        Incrementors.Add(OperationFactory.GetExpressionList(incrementOperator, container));
      }

      // 条件部
      var condition = semanticModel.GetOperation(node.Condition);
      Conditions.AddRange(OperationFactory.GetExpressionList(condition, container));

      // 内部処理設定
      var block = node.Statement as BlockSyntax;
      foreach (var statement in block.Statements)
      {
        Members.Add(ItemFactory.Create(statement, semanticModel, container, this));
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

      result.Append(indexSpace);
      result.Append("for(");

      // 宣言
      var declarations = new List<string>();
      Declarations.ForEach(items => 
      {
        var declaration = new StringBuilder();
        items.ForEach(item => declaration.Append(item.Name));
        declarations.Add(declaration.ToString());
      });
      result.Append(string.Join(",",declarations.ToArray()));
      result.Append(";");

      for (var i = 0; i < Conditions.Count; i++)
      {
        var isSetSpace = true;
        if (i == Conditions.Count - 1)
        {
          isSetSpace = false;
        }
        else if (Conditions[i].Name == "." || Conditions[i + 1].Name == ".")
        {
          isSetSpace = false;
        }
        result.Append($"{Conditions[i].Name}");
        if (isSetSpace)
        {
          result.Append(" ");
        }
      }
      result.Append(";");

      var incrementors = new List<string>();
      Incrementors.ForEach(items =>
      {
        var incrementor = new StringBuilder();
        items.ForEach(item => incrementor.Append(item.Name));
        incrementors.Add(incrementor.ToString());
      });
      result.Append(string.Join(",", incrementors.ToArray()));
      result.AppendLine(")");

      result.Append(indexSpace);
      result.AppendLine("{");
      foreach (var statement in Members)
      {
        result.AppendLine(statement.ToString(index + 1));
      }
      result.Append(indexSpace);
      result.AppendLine("}");

      return result.ToString();
    }

    #endregion
  }
}
