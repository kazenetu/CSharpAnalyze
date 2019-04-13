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
  /// アイテム：列挙型
  /// </summary>
  internal class ItemEnum : AbstractItem, IItemEnum
  {
    /// <summary>
    /// 列挙型の値リスト
    /// </summary>
    /// <remarks>名称と初期値のリスト</remarks>
    public Dictionary<string, string> Items { get; } = new Dictionary<string, string>();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="node">対象Node</param>
    /// <param name="semanticModel">対象ソースのsemanticModel</param>
    /// <param name="parent">親IAnalyzeItem</param>
    /// <param name="container">イベントコンテナ</param>
    public ItemEnum(EnumDeclarationSyntax node, SemanticModel semanticModel, IAnalyzeItem parent, EventContainer container) : base(parent, node, semanticModel, container)
    {
      ItemType = ItemTypes.Enum;

      // アイテムと初期値を取得
      foreach(var member in node.Members)
      {
        var declaredSymbol = semanticModel.GetDeclaredSymbol(member) as IFieldSymbol;
        var name = declaredSymbol.Name;
        var value = declaredSymbol.ConstantValue;
        Items.Add(name, value?.ToString());
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

      result.AppendLine($"enum {Name} {{");

      // 列挙型の値リスト
      var listCount = Items.Keys.Count;
      var itemIndexSpace = string.Concat(Enumerable.Repeat("  ", index + 1));
      foreach (var item in Items)
      {
        result.Append(itemIndexSpace);
        result.Append(item.Key);

        // 初期値設定
        if(item.Value != null)
        {
          result.Append($" = {item.Value}");
        }

        if (--listCount > 0)
        {
          result.Append(",");
        }
        result.AppendLine("");
      }

      result.Append(indexSpace);
      result.AppendLine("}");

      return result.ToString();
    }

    #endregion
  }
}
