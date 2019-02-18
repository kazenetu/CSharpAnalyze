using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpAnalyzeTest.Common
{
  /// <summary>
  /// ファイルデータ
  /// </summary>
  public class FileData
  {
    #region プロパティ

    /// <summary>
    /// ファイルパス
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// 追加using
    /// </summary>
    public string AddUsing { get; }

    /// <summary>
    /// ソースコード
    /// </summary>
    public string Source { get; }

    #endregion

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="filePath">ファイルパス</param>
    /// <param name="addUsing">追加using</param>
    /// <param name="source">ソースコード</param>
    public FileData(string filePath, string addUsing, string source)
    {
      FilePath = filePath;
      AddUsing = addUsing;
      Source = source;
    }
  }
}
