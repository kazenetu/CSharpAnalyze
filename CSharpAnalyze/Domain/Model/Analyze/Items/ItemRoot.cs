using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpAnalyze.Domain.Model.Analyze.Items
{
  /// <summary>
  /// アイテム：ファイルルート
  /// </summary>
  /// <remarks>ファイルルートクラスに昇格予定</remarks>
  [Obsolete]
  public class ItemRoot : AbstractItem
  {
    /// <summary>
    /// 外部参照のクラス名とファイルパスのリスト
    /// </summary>
    public Dictionary<string, string> OtherFiles { get; } = new Dictionary<string, string>();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ItemRoot()
    {
      ItemType = ItemTypes.Root;
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

      // 外部参照ファイル
      foreach (var otherFile in OtherFiles)
      {
        result.AppendLine($"OtherFileReference：[{otherFile.Key}] in [{otherFile.Value}]");
      }

      // メンバー
      Members.ForEach(member => result.AppendLine(member.ToString(index)));
      return result.ToString();
    }

    #endregion

  }
}
