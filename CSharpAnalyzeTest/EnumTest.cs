using CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems;
using CSharpAnalyzeTest.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CSharpAnalyzeTest
{
  [Trait("列挙型のテスト", nameof(EnumTest))]
  public class EnumTest : TestBase
  {
    /// <summary>
    /// ソース作成パターン
    /// </summary>
    private enum CreatePattern
    {
      Standard,
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
          filePath = "Standard.cs";

          source.AppendLine("public class StandardTest");
          source.AppendLine("{");
          source.AppendLine("  private enum CreatePattern");
          source.AppendLine("  {");
          source.AppendLine("    First,");
          source.AppendLine("    Second,");
          source.AppendLine("    Third");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;
      }

      return new FileData(filePath, usingList.ToString(), source.ToString());
    }

    /// <summary>
    /// Setup
    /// </summary>
    public EnumTest() : base()
    {
      System.Diagnostics.Debug.WriteLine($"Setup {Environment.CurrentDirectory}");
    }

    /// <summary>
    /// 列挙型のスタンダードなテスト
    /// </summary>
    [Fact(DisplayName = "Standard")]
    public void StandardTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.Standard), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "Standard.cs");

        // メンバー数確認
        Assert.Single(itemClass.Members);

        // 列挙型であることを確認
        Assert.IsAssignableFrom<IItemEnum>(itemClass.Members[0]);

        var itemEmun = itemClass.Members[0] as IItemEnum;

        // アクセス修飾子の確認
        var modifiers = new List<string>() { "private" };
        Assert.Equal(modifiers, itemEmun.Modifiers);

        // 要素の確認
        var items = new Dictionary<string,string>()
        {
          { "First","0" },
          { "Second","1"},
          { "Third","2"},
        };
        Assert.Equal(items, itemEmun.Items);

      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }



  }
}
