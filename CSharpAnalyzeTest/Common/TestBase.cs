﻿using CSharpAnalyze.ApplicationService;
using CSharpAnalyze.Domain.PublicInterfaces;
using CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems;
using CSharpAnalyze.Domain.PublicInterfaces.Events;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xunit;

namespace CSharpAnalyzeTest.Common
{
  /// <summary>
  /// テストのスーパークラス
  /// </summary>
  public class TestBase : IDisposable
  {
    /// <summary>
    /// テスト対象
    /// </summary>
    protected AnalyzeApplication CSAnalyze = new AnalyzeApplication();

    /// <summary>
    /// テスト用FileRepository
    /// </summary>
    protected CSFileRepositoryMock Files = new CSFileRepositoryMock();

    /// <summary>
    /// ソースコード
    /// </summary>
    private string BaseSource = string.Empty;

    /// <summary>
    /// Setup
    /// </summary>
    public TestBase()
    {
      // 基本ソースの組み立て
      var baseSource = new StringBuilder();
      baseSource.AppendLine("using System;");
      baseSource.AppendLine("using System.Collections.Generic;");
      baseSource.AppendLine("{0}");
      baseSource.AppendLine("namespace CSharpAnalyzeTest{{");
      baseSource.AppendLine("{1}");
      baseSource.AppendLine("}}");
      BaseSource = baseSource.ToString();

      // 解析終了時に呼ばれるイベントを登録
      CSAnalyze.Register<IAnalyzed>(CSAnalyze, (ev) =>
      {
        var method = Files.GetDelegateMethod(ev.FilePath);
        if (method == null) return;

        method(ev);
      });
    }

    /// <summary>
    /// Teardown
    /// </summary>
    public void Dispose()
    {
      GC.SuppressFinalize(this);
    }

    #region テスト情報作成メソッド

    /// <summary>
    /// テスト情報作成
    /// </summary>
    /// <param name="fileName">ファイル名</param>
    /// <param name="addUsing">追加Using</param>
    /// <param name="sourceCode">ソースコード</param>
    /// <param name="delegateMethod">イベント処理</param>
    protected void CreateFileData(string fileName,string addUsing, string sourceCode, Action<IAnalyzed> delegateMethod)
    {
      var source = string.Format(CultureInfo.CurrentCulture, BaseSource, addUsing, sourceCode, delegateMethod);
      Files.Add(fileName, source, delegateMethod);
    }

    /// <summary>
    /// テスト情報作成
    /// </summary>
    /// <param name="fileData">テスト情報作成</param>
    /// <param name="delegateMethod">イベント処理</param>
    protected void CreateFileData(FileData fileData, Action<IAnalyzed> delegateMethod)
    {
      var source = string.Format(CultureInfo.CurrentCulture, BaseSource, fileData.AddUsing, fileData.Source, delegateMethod);
      Files.Add(fileData.FilePath, source, delegateMethod);
    }

    #endregion

    #region ユーティリティ

    /// <summary>
    /// クラスインターフェースインスタンスの取得
    /// </summary>
    /// <param name="ev">解析結果イベントインスタンス</param>
    /// <param name="filePath">ファイル名</param>
    /// <param name="classIndex">取得対象インデックス(初期値:0)</param>
    /// <returns>クラスインターフェースインスタンス</returns>
    protected IItemClass GetClassInstance(IAnalyzed ev, string filePath, int classIndex = 0)
    {
      // ファイル名の確認
      Assert.Equal(ev.FilePath, filePath);

      // 解析結果の存在確認
      Assert.NotNull(ev.FileRoot);

      // 解析結果の件数確認
      Assert.True(ev.FileRoot.Members.Count >= classIndex + 1, $"{ev.FileRoot.Members.Count} < {classIndex + 1}");

      // IItemClassインスタンスの確認
      Assert.NotNull(ev.FileRoot.Members[classIndex] as IItemClass);

      // IItemClassインスタンスを返す
      return ev.FileRoot.Members[classIndex] as IItemClass;
    }

    /// <summary>
    /// Expressionリストを文字列に変換する
    /// </summary>
    /// <param name="expressions">Expressionリスト</param>
    /// <returns></returns>
    protected string GetExpressionsToString(List<IExpression> expressions)
    {
      return string.Concat(expressions.Select(expression => spacer(expression.Name)));

      // 単語の最後にスペースを入れるか確認し返す
      string spacer(string src)
      {
        bool addPrefixSpace = false;
        bool addSuffixSpace = false;

        switch (src)
        {
          case "new":
            addSuffixSpace = true;
            break;
          case "is":
            addPrefixSpace = true;
            addSuffixSpace = true;
            break;
        }

        if (addPrefixSpace){
          src = " " + src;
        }

        if (addSuffixSpace)
        {
          src += " ";
        }

        return src;
      }
    }

    #endregion

  }
}
