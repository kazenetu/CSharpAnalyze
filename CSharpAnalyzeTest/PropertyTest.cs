using CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems;
using CSharpAnalyzeTest.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CSharpAnalyzeTest
{
  [Trait("プロパティのテスト", nameof(PropertyTest))]
  public class PropertyTest : TestBase
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

          source.AppendLine("public class ClassTest");
          source.AppendLine("{");
          source.AppendLine("  public string PropertyString{set; get;}");
          source.AppendLine("  public int PropertydInt{get;} = 1;");
          source.AppendLine("}");
          break;

      return new FileData(filePath, usingList.ToString(), source.ToString());
    }

    /// <summary>
    /// Setup
    /// </summary>
    public PropertyTest() : base()
    {
      System.Diagnostics.Debug.WriteLine($"Setup {Environment.CurrentDirectory}");
    }

    /// <summary>
    /// スタンダードなプロパティのテスト
    /// </summary>
    [Fact(DisplayName = "Standard")]
    public void StandardTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.Standard), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "Standard.cs");

        // クラス内の要素の存在確認
        var expectedList = new List<(List<string> modifiers, string name, string type, List<string> accessors, bool isInit, List<string> init)>
           {
             (new List<string>() { "public" }, "PropertyString", "string",new List<string>(){"set","get"} , false, null),
             (new List<string>() { "public" }, "PropertydInt", "int",new List<string>(){"get"}, true, new List<string>() { "1" }),
           };
        Assert.Equal(expectedList.Count, GetMemberCount(itemClass, expectedList));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }


    /// <summary>
    /// メンバー数を取得
    /// </summary>
    /// <param name="itemClass">対象のアイテムクラス</param>
    /// <param name="expectedList">予想値リスト</param>
    /// <returns>条件が一致するメンバー数</returns>
    private int GetMemberCount(IItemClass itemClass, List<(List<string> modifiers, string name, string type, List<string> accessors, bool isInit, List<string> init)> expectedList)
    {
      var memberCount = 0;
      foreach (var member in itemClass.Members)
      {
        // 対象以外は次のmemberへ
        if (!(member is IItemProperty memberProperty)) continue;

        // 型の取得
        var memberFieldType = new StringBuilder();
        memberProperty.PropertyTypes.ForEach(item => memberFieldType.Append(item.Name));

        // 型の一致確認
        var targetProperties = expectedList.Where(field => field.name == memberProperty.Name && field.type == memberFieldType.ToString());
        if (!targetProperties.Any()) continue;

        // 条件取得
        var (modifiers, name, type, accessors, isInit, init) = targetProperties.First();

        // アクセサの一致確認
        Assert.Equal(accessors, memberProperty.AccessorList);

        // アクセス修飾子の確認
        Assert.Equal(modifiers, memberProperty.Modifiers);

        // 初期値が設定されている
        if (isInit)
        {
          // 初期値の数が一致しない場合は次のmemberへ
          if (memberProperty.DefaultValues.Count != init.Count) continue;

          // 初期値のコレクションと条件のコレクションの一致確認
          var defaultValues = memberProperty.DefaultValues.Select(value => value.Name).ToList();
          Assert.Equal(init, defaultValues);
        }

        memberCount++;
      }

      return memberCount;
    }
  }
}
