using CSharpAnalyze.Domain.PublicInterfaces;
using CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems;
using CSharpAnalyzeTest.Common;
using System;
using System.Linq;
using System.Text;
using Xunit;

namespace CSharpAnalyzeTest
{
  [Trait("クラスのテスト", nameof(ClassTest))]
  public class ClassTest : TestBase
  {
    /// <summary>
    /// ソース作成パターン
    /// </summary>
    private enum CreatePattern
    {
      Standard,
      SubClass,
      InnerClass
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
        case CreatePattern.Standard:
          filePath = "Test.cs";

          source.AppendLine("public class ClassTest");
          source.AppendLine("{");
          source.AppendLine("}");
          break;

        case CreatePattern.SubClass:
          filePath = "SubClass.cs";

          source.AppendLine("public class SubClass : ClassTest");
          source.AppendLine("{");
          source.AppendLine("}");
          break;

        case CreatePattern.InnerClass:
          filePath = "InnerClass.cs";

          source.AppendLine("public class ClassTest ");
          source.AppendLine("{");
          source.AppendLine("  private static class InnerClass ");
          source.AppendLine("  {");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;
      }

      return new FileData(filePath, usingList.ToString(), source.ToString());
    }

    /// <summary>
    /// Setup
    /// </summary>
    public ClassTest() : base()
    {
      System.Diagnostics.Debug.WriteLine($"Setup {Environment.CurrentDirectory}");
    }

    /// <summary>
    /// 基本的なクラステスト
    /// </summary>
    [Fact(DisplayName = "Standard")]
    public void StandardTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.Standard), (ev) =>
         {
           // IItemClassインスタンスを取得
           var itemClass = GetClassInstance(ev, "Test.cs", 0);

           // スーパークラスの設定確認
           Assert.True(itemClass.SuperClass.Count == 0);

           // 親の存在確認
           Assert.Null(itemClass.Parent);

           // クラス名の確認
           Assert.True(itemClass.Name == "ClassTest");

           // スコープ修飾子の件数確認
           Assert.True(itemClass.Modifiers.Count == 1);

           // スコープ修飾子の内容確認
           Assert.Contains("public", itemClass.Modifiers);

           // ItemTypeの確認
           Assert.True(itemClass.ItemType == ItemTypes.Class);

           // クラス内の要素の存在確認
           Assert.True(itemClass.Members.Count == 0);
         });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// サブクラスのテスト
    /// </summary>
    [Fact(DisplayName = "SubClass")]
    public void SubClassTest()
    {
      // スーパークラスを追加
      CreateFileData(CreateSource(CreatePattern.Standard), null);

      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.SubClass), (ev) =>
         {
           // IItemClassインスタンスを取得
           var itemClass = GetClassInstance(ev, "SubClass.cs", 0);

           // 外部参照の存在確認
           Assert.True(ev.FileRoot.OtherFiles.Count == 1);
           Assert.True(ev.FileRoot.OtherFiles.First().Key == "ClassTest");
           Assert.True(ev.FileRoot.OtherFiles.First().Value == "Test.cs");

           // 解析結果の件数確認
           Assert.True(ev.FileRoot.Members.Count == 1);

           // スーパークラスの設定確認
           Assert.True(itemClass.SuperClass.Count == 1);
           Assert.True(itemClass.SuperClass.First().Name == "ClassTest");

           // 親の存在確認
           Assert.Null(itemClass.Parent);

           // クラス名の確認
           Assert.True(itemClass.Name == "SubClass");

           // スコープ修飾子の件数確認
           Assert.True(itemClass.Modifiers.Count == 1);

           // スコープ修飾子の内容確認
           Assert.Contains("public", itemClass.Modifiers);

           // ItemTypeの確認
           Assert.True(itemClass.ItemType == ItemTypes.Class);

           // クラス内の要素の存在確認
           Assert.True(itemClass.Members.Count == 0);
         });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// 内部クラスのテスト
    /// </summary>
    [Fact(DisplayName = "InnerClass")]
    public void InnerClassTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.InnerClass), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "InnerClass.cs", 0);

        // 外部参照の存在確認
        Assert.True(ev.FileRoot.OtherFiles.Count == 0);

        // スーパークラスの設定確認
        Assert.True(itemClass.SuperClass.Count == 0);

        // 親の存在確認
        Assert.Null(itemClass.Parent);

        // クラス名の確認
        Assert.True(itemClass.Name == "ClassTest");

        // スコープ修飾子の件数確認
        Assert.True(itemClass.Modifiers.Count == 1);

        // スコープ修飾子の内容確認
        Assert.Contains("public", itemClass.Modifiers);

        // ItemTypeの確認
        Assert.True(itemClass.ItemType == ItemTypes.Class);

        // クラス内の要素の存在確認
        Assert.True(itemClass.Members.Count == 1);

        #region 内部クラスの確認

        // クラス内の要素がクラス要素である確認
        Assert.True(itemClass.Members[0] is IItemClass);
        var innerClass = itemClass.Members[0] as IItemClass;
        Assert.True(innerClass.Name == "InnerClass");

        // スコープ修飾子の件数確認
        Assert.True(innerClass.Modifiers.Count == 2);

        // スコープ修飾子の内容確認
        Assert.Contains("private", innerClass.Modifiers);
        Assert.Contains("static", innerClass.Modifiers);

        // ItemTypeの確認
        Assert.True(innerClass.ItemType == ItemTypes.Class);

        // クラス内の要素の存在確認
        Assert.True(innerClass.Members.Count == 0);

        #endregion
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

  }
}
