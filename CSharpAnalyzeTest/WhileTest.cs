using CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems;
using CSharpAnalyzeTest.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CSharpAnalyzeTest
{
  [Trait("Whileのテスト", nameof(WhileTest))]
  public class WhileTest : TestBase
  {
    /// <summary>
    /// ソース作成パターン
    /// </summary>
    private enum CreatePattern
    {
      Standard,
      DoWhile,
      Increment,
      Decrement,

      // TODO 実装
      InstanceProperty,
      InstanceMethod,
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
      var addMember = new List<string>();
      var addSource = new StringBuilder();

      switch (pattern)
      {
        case CreatePattern.Standard:
          filePath = "Standard.cs";

          source.Add("var val=0;");
          source.Add("while(val < 10)");
          source.Add("{");
          source.Add("  val++;");
          source.Add("}");
          break;

        case CreatePattern.DoWhile:
          filePath = "DoWhile.cs";

          source.Add("var val=0;");
          source.Add("do{");
          source.Add("  val++;");
          source.Add("}");
          source.Add("while(val < 10)");
          break;

        case CreatePattern.Increment:
          filePath = "Increment.cs";

          source.Add("var val=0;");
          source.Add("while(val++ < 10)");
          source.Add("{");
          source.Add("}");
          break;

        case CreatePattern.Decrement:
          filePath = "Decrement.cs";

          source.Add("var val=10;");
          source.Add("do{");
          source.Add("}");
          source.Add("while(--val >= 0)");
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

      addMember.ForEach(line => targetSource.AppendLine($"  {line }"));
      targetSource.AppendLine("}");
      targetSource.AppendLine(addSource.ToString());

      return new FileData(filePath, usingList.ToString(), targetSource.ToString());
    }

    /// <summary>
    /// Setup
    /// </summary>
    public WhileTest() : base()
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
        var targetInstance = GetTargetInstances<IItemWhile>(targetParentInstance).First();

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 条件の確認
        Assert.Equal("val<10", GetExpressionsToString(targetInstance.Conditions));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// do-whileテスト
    /// </summary>
    [Fact(DisplayName = "DoWhile")]
    public void DoWhileTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.DoWhile), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "DoWhile.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances<IItemDo>(targetParentInstance).First();

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 条件の確認
        Assert.Equal("val<10", GetExpressionsToString(targetInstance.Conditions));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// 条件：インクリメントのテスト
    /// </summary>
    [Fact(DisplayName = "Increment")]
    public void IncrementTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.Increment), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "Increment.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances<IItemWhile>(targetParentInstance).First();

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 条件の確認
        Assert.Equal("val++<10", GetExpressionsToString(targetInstance.Conditions));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// 条件：デクリメントのテスト
    /// </summary>
    [Fact(DisplayName = "Decrement")]
    public void DecrementTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.Decrement), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "Decrement.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances<IItemDo>(targetParentInstance).First();

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 条件の確認
        Assert.Equal("--val>=0", GetExpressionsToString(targetInstance.Conditions));
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
    private List<T> GetTargetInstances<T>(IItemMethod itemMethod) where T:class
    {
      return itemMethod.Members.Where(member => member is T).
              Select(member => member as T).ToList();
    }

    #endregion

  }
}
