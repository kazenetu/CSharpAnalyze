using CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems;
using CSharpAnalyzeTest.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CSharpAnalyzeTest
{
  [Trait("ForEachのテスト", nameof(ForEachTest))]
  public class ForEachTest : TestBase
  {
    /// <summary>
    /// ソース作成パターン
    /// </summary>
    private enum CreatePattern
    {
      Standard,
      ListStrings,
    }

    /// <summary>
    /// ファイル名、ソースコード取得処理
    /// </summary>
    /// <param name="pattern">生成パターン</param>
    /// <returns>ファイルパスとソースコード</returns>
    private FileData CreateSource(CreatePattern pattern)
    {
      var usingList = new StringBuilder();
      var source = new List<string>();
      var filePath = string.Empty;

      switch (pattern)
      {
        case CreatePattern.Standard:
          filePath = "Standard.cs";

          source.Add("var values = new int[]{1,2,3 };");
          source.Add("foreach(var value in values){");
          source.Add("{");
          source.Add("}");
          break;

        case CreatePattern.ListStrings:
          filePath = "ListStrings.cs";

          source.Add("var values = new List<string>{\"1\",\"2\", \"3\" };");
          source.Add("foreach(var value in values){");
          source.Add("{");
          source.Add("}");
          break;
      }

      // ソースコード作成
      var targetSource = new StringBuilder();
      targetSource.AppendLine("public class Standard");
      targetSource.AppendLine("{");
      targetSource.AppendLine("  public void TestMethod()");
      targetSource.AppendLine("  {");

      source.ForEach(line => targetSource.AppendLine($"    {line }"));

      targetSource.AppendLine("  }");
      targetSource.AppendLine("}");

      return new FileData(filePath, usingList.ToString(), targetSource.ToString());
    }

    /// <summary>
    /// Setup
    /// </summary>
    public ForEachTest() : base()
    {
      System.Diagnostics.Debug.WriteLine($"Setup {Environment.CurrentDirectory}");
    }

    /// <summary>
    /// 基本的なテスト
    /// </summary>
    [Fact(DisplayName = "Standard")]
    public void StandardTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.Standard), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "Standard.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemForEach;

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // ローカル部の型の確認
        Assert.Equal("int", GetExpressionsToString(targetInstance.LocalTypes));

        // ローカル部の確認
        Assert.Equal("value", GetExpressionsToString(targetInstance.Local));

        // コレクション部の型の確認
        Assert.Equal("int[]", GetExpressionsToString(targetInstance.CollectionTypes));

        // コレクション部部の確認
        Assert.Equal("values", GetExpressionsToString(targetInstance.Collection));

      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// Listジェネリクスのテスト
    /// </summary>
    [Fact(DisplayName = "ListStrings")]
    public void ListStringsTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.ListStrings), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "ListStrings.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemForEach;

        // 外部参照の存在確認
        Assert.Single(ev.FileRoot.OtherFiles);
        Assert.Equal("List",ev.FileRoot.OtherFiles.First().Key);

        // ローカル部の型の確認
        Assert.Equal("string", GetExpressionsToString(targetInstance.LocalTypes));

        // ローカル部の確認
        Assert.Equal("value", GetExpressionsToString(targetInstance.Local));

        // コレクション部の型の確認
        Assert.Equal("List<string>", GetExpressionsToString(targetInstance.CollectionTypes));

        // コレクション部部の確認
        Assert.Equal("values", GetExpressionsToString(targetInstance.Collection));

      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    #region ユーティリティメソッド
    /// <summary>
    /// 対象インスタンスの取得
    /// </summary>
    /// <param name="itemClass">対象のアイテムクラス</param>
    /// <returns>対象インスタンスリスト</returns>
    private List<IItemMethod> GetTargetInstances(IItemClass itemClass)
    {
      return itemClass.Members.Where(member => member is IItemMethod).
              Select(member => member as IItemMethod).ToList();
    }

    /// <summary>
    /// 対象インスタンスの取得
    /// </summary>
    /// <param name="itemClass">対象のアイテムメソッド</param>
    /// <returns>対象インスタンスリスト</returns>
    private List<IItemForEach> GetTargetInstances(IItemMethod itemMethod)
    {
      return itemMethod.Members.Where(member => member is IItemForEach).
              Select(member => member as IItemForEach).ToList();
    }
    #endregion
  }
}
