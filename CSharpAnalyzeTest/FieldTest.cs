using CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems;
using CSharpAnalyzeTest.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CSharpAnalyzeTest
{
  [Trait("フィールドのテスト", nameof(FieldTest))]
  public class FieldTest : TestBase
  {
    /// <summary>
    /// ソース作成パターン
    /// </summary>
    private enum CreatePattern
    {
      Standard,
      ClassField,
      ListField
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
          source.AppendLine("  public string FieldString;");
          source.AppendLine("  public int FieldInt = 1;");
          source.AppendLine(@"  private const string Const=""123"";");
          source.AppendLine("}");
          break;

        case CreatePattern.ClassField:
          filePath = "ClassField.cs";

          source.AppendLine("public class ClassField");
          source.AppendLine("{");
          source.AppendLine("  private ClassTest fieldClass1;");
          source.AppendLine("  protected ClassTest fieldClass2 = new ClassTest();");
          source.AppendLine("  private static ClassTest fieldClass3 = null;");
          source.AppendLine("}");
          break;

        case CreatePattern.ListField:
          filePath = "ListField.cs";

          source.AppendLine("public class ListField");
          source.AppendLine("{");
          source.AppendLine("  private List<string> field1;");
          source.AppendLine("  private List<string> field2 = new List<string>();");
          source.AppendLine("}");
          break;
      }

      return new FileData(filePath, usingList.ToString(), source.ToString());
    }

    /// <summary>
    /// Setup
    /// </summary>
    public FieldTest() : base()
    {
      System.Diagnostics.Debug.WriteLine($"Setup {Environment.CurrentDirectory}");
    }

    /// <summary>
    /// 組み込み型フィールドのテスト
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
           var expectedList = new List<(List<string> modifiers, string name, string type, bool isInit, List<string> init)>
           {
             (new List<string>() { "public" }, "FieldString", "string", false, null),
             (new List<string>() { "public" }, "FieldInt", "int", true, new List<string>() { "1" }),
             (new List<string>() { "private","const" }, "Const", "string", true, new List<string>() { "\"123\"" })
           };
           Assert.Equal(expectedList.Count, GetMemberCount(itemClass, expectedList));
         });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// クラス型フィールドのテスト
    /// </summary>
    [Fact(DisplayName = "ClassField")]
    public void ClassFieldTest()
    {
      // スーパークラスを追加
      CreateFileData(CreateSource(CreatePattern.Standard), null);

      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.ClassField), (ev) =>
         {
           // IItemClassインスタンスを取得
           var itemClass = GetClassInstance(ev, "ClassField.cs");

           // 外部参照の存在確認
           Assert.Single(ev.FileRoot.OtherFiles);
           Assert.Equal("ClassTest", ev.FileRoot.OtherFiles.First().Key);
           Assert.Equal("Standard.cs", ev.FileRoot.OtherFiles.First().Value);

           // クラス内の要素の存在確認
           var expectedList = new List<(List<string> modifiers, string name, string type, bool isInit, List<string> init)>
           {
             (new List<string>() { "private" }, "fieldClass1", "ClassTest", false, null),
             (new List<string>() { "protected" }, "fieldClass2", "ClassTest", true, new List<string>() { "new", "ClassTest", "(", ")" }),
             (new List<string>() { "private", "static" }, "fieldClass3", "ClassTest", true, new List<string>() { "null" })
           };
           Assert.Equal(expectedList.Count , GetMemberCount(itemClass, expectedList));
         });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// 組み込みジェネリッククラスフィールドのテスト
    /// </summary>
    [Fact(DisplayName = "ListField")]
    public void ListFieldTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.ListField), (ev) =>
         {
           // IItemClassインスタンスを取得
           var itemClass = GetClassInstance(ev, "ListField.cs");

           // 外部参照の存在確認
           Assert.Single(ev.FileRoot.OtherFiles);
           Assert.Equal("List", ev.FileRoot.OtherFiles.First().Key);
           Assert.Equal(string.Empty, ev.FileRoot.OtherFiles.First().Value);

           // クラス内の要素の存在確認
           var expectedList = new List<(List<string> modifiers, string name, string type, bool isInit, List<string> init)>
           {
             (new List<string>() { "private" }, "field1", "List<string>", false, null),
             (new List<string>() { "private" }, "field2", "List<string>", true, new List<string>() { "new", "List","<","string",">", "(", ")" })
           };
           Assert.Equal(expectedList.Count , GetMemberCount(itemClass, expectedList));
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
    private int GetMemberCount(IItemClass itemClass, List<(List<string> modifiers, string name, string type, bool isInit, List<string> init)> expectedList)
    {
      var memberCount = 0;
      foreach (var member in itemClass.Members)
      {
        // フィールド以外は次のmemberへ
        if (!(member is IItemField memberField)) continue;

        // 型の取得
        var memberFieldType = new StringBuilder();
        memberField.FieldTypes.ForEach(item => memberFieldType.Append(item.Name));

        // 型の一致確認
        var targetFileds = expectedList.Where(field => field.name == memberField.Name && field.type == memberFieldType.ToString());
        if (!targetFileds.Any()) continue;

        // 条件取得
        var (modifiers, name, type, isInit, init) = targetFileds.First();

        // アクセス修飾子の確認
        Assert.Equal(modifiers, memberField.Modifiers);

        // 初期値が設定されている
        if (isInit)
        {
          // 初期値の数が一致しない場合は次のmemberへ
          if (memberField.DefaultValues.Count != init.Count) continue;

          // 初期値のコレクションと条件のコレクションの一致確認
          var defaultValues = memberField.DefaultValues.Select(value => value.Name).ToList();
          Assert.Equal(init, defaultValues);
        }

        memberCount++;
      }

      return memberCount;
    }

  }
}
