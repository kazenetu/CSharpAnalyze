using CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems;
using CSharpAnalyzeTest.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CSharpAnalyzeTest
{
  [Trait("Returnのテスト", nameof(ReturnTest))]
  public class ReturnTest : TestBase
  {
    /// <summary>
    /// ソース作成パターン
    /// </summary>
    private enum CreatePattern
    {
      ReturnVoid,
      ReturnInt,
      ReturnString,
      ReturnInstance,
      ReturnGenericInstance,
    }

    /// <summary>
    /// ファイル名、ソースコード取得処理
    /// </summary>
    /// <param name="pattern">生成パターン</param>
    /// <returns>ファイルパスとソースコード</returns>
    private FileData CreateSource(CreatePattern pattern)
    {
      var usingList = new StringBuilder();
      var source = new StringBuilder();
      var filePath = string.Empty;

      switch (pattern)
      {
        case CreatePattern.ReturnVoid:
          filePath = "ReturnVoid.cs";

          source.AppendLine("public class ReturnValue");
          source.AppendLine("{");
          source.AppendLine("  public void TestMethod()");
          source.AppendLine("  {");
          source.AppendLine("    return;");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;

        case CreatePattern.ReturnInt:
          filePath = "ReturnInt.cs";

          source.AppendLine("public class ReturnValue");
          source.AppendLine("{");
          source.AppendLine("  public int TestMethod()");
          source.AppendLine("  {");
          source.AppendLine("    return 10;");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;
      }

      return new FileData(filePath, usingList.ToString(), source.ToString());
    }

    /// <summary>
    /// Setup
    /// </summary>
    public ReturnTest() : base()
    {
      System.Diagnostics.Debug.WriteLine($"Setup {Environment.CurrentDirectory}");
    }

    /// <summary>
    /// 値を返さないテスト
    /// </summary>
    [Fact(DisplayName = "ReturnVoid")]
    public void ReturnVoidTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.ReturnVoid), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "ReturnVoid.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).FirstOrDefault();
        Assert.NotNull(targetInstance);

        // 戻り値の確認
        Assert.Equal("", GetExpressionsToString(targetInstance.ReturnValue));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// int値を返すテスト
    /// </summary>
    [Fact(DisplayName = "ReturnInt")]
    public void ReturnIntTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.ReturnInt), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "ReturnInt.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).FirstOrDefault();
        Assert.NotNull(targetInstance);

        // 戻り値の確認
        Assert.Equal("10", GetExpressionsToString(targetInstance.ReturnValue));
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
    private List<IItemReturn> GetTargetInstances(IItemMethod itemMethod)
    {
      return itemMethod.Members.Where(member => member is IItemReturn).
              Select(member => member as IItemReturn).ToList();
    }

    #endregion
  }
}
