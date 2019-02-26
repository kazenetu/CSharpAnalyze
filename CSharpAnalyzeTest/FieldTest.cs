using CSharpAnalyze.Domain.PublicInterfaces;
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
      ClassField
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
          source.AppendLine("  public string fieldString;");
          source.AppendLine("  public int fieldInt = 1;");
          source.AppendLine("}");
          break;

        case CreatePattern.ClassField:
          filePath = "ClassField.cs";

          //usingList.AppendLine("using ClassTest;");

          source.AppendLine("public class ClassField");
          source.AppendLine("{");
          source.AppendLine("  public ClassTest fieldClass1;");
          source.AppendLine("  public ClassTest fieldClass2 = new ClassTest();");
          source.AppendLine("  public ClassTest fieldClass3 = null;");
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

    [Fact(DisplayName = "Standard")]
    public void StandardTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.Standard), (ev) =>
         {
           // ファイル名の確認
           Assert.True(ev.FilePath == "Standard.cs");

           // 解析結果の存在確認
           Assert.NotNull(ev.FileRoot);

           // 外部参照の存在確認
           Assert.True(ev.FileRoot.OtherFiles.Count == 0);

           // 解析結果の件数確認
           Assert.True(ev.FileRoot.Members.Count == 1);

           // IItemClassインスタンスの確認
           Assert.True(ev.FileRoot.Members[0] is IItemClass);

           // IItemClassインスタンスを取得
           var itemClass = ev.FileRoot.Members[0] as IItemClass;

           // クラス内の要素の存在確認
           var fields = new List<(string name, string type, bool isInit, List<string> init)>
           {
             ("fieldString", "string", false, null),
             ("fieldInt", "int", true, new List<string>() { "1" })
           };

           Assert.True(itemClass.Members.Count == GetMemberCount(itemClass, fields));
         });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    [Fact(DisplayName = "ClassField")]
    public void ClassFieldTest()
    {
      // スーパークラスを追加
      CreateFileData(CreateSource(CreatePattern.Standard), null);

      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.ClassField), (ev) =>
         {
           // ファイル名の確認
           Assert.True(ev.FilePath == "ClassField.cs");

           // 解析結果の存在確認
           Assert.NotNull(ev.FileRoot);

           // 外部参照の存在確認
           Assert.True(ev.FileRoot.OtherFiles.Count == 1);
           Assert.True(ev.FileRoot.OtherFiles.First().Key == "ClassTest");
           Assert.True(ev.FileRoot.OtherFiles.First().Value == "Standard.cs");

           // 解析結果の件数確認
           Assert.True(ev.FileRoot.Members.Count == 1);

           // IItemClassインスタンスの確認
           Assert.True(ev.FileRoot.Members[0] is IItemClass);

           // IItemClassインスタンスを取得
           var itemClass = ev.FileRoot.Members[0] as IItemClass;

           // クラス内の要素の存在確認
           var fields = new List<(string name, string type, bool isInit, List<string> init)>
           {
             ("fieldClass1", "ClassTest", false, null),
             ("fieldClass2", "ClassTest", true, new List<string>() { "new", "ClassTest", "(", ")" }),
             ("fieldClass3", "ClassTest", true, new List<string>() { "null" })
           };
           Assert.True(itemClass.Members.Count == GetMemberCount(itemClass, fields));

         });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// メンバー数を取得
    /// </summary>
    /// <param name="itemClass">対象のアイテムクラス</param>
    /// <param name="condition">条件</param>
    /// <returns>条件が一致するメンバー数</returns>
    private int GetMemberCount(IItemClass itemClass, List<(string name, string type, bool isInit, List<string> init)> condition)
    {
      var memberCount = 0;
      foreach (var member in itemClass.Members)
      {
        // フィールド以外は次のmemberへ
        if (!(member is IItemField memberField)) continue;

        // 型の取得と一致確認
        var memberFieldType = new StringBuilder();
        memberField.FieldTypes.ForEach(item => memberFieldType.Append(item.Name));
        var targetFileds = condition.Where(field => field.name == memberField.Name && field.type == memberFieldType.ToString());
        if (!targetFileds.Any()) continue;

        // 条件取得
        var (name, type, isInit, init) = targetFileds.First();

        // 初期値が設定されている
        if (isInit)
        {
          // 初期値の数が一致しない場合は次のmemberへ
          if (memberField.DefaultValues.Count != init.Count) continue;

          // 初期値のコレクションと条件のコレクションの一致確認
          var defaultValueIndex = 0;
          memberField.DefaultValues.ForEach(value => { if (value.Name == init[defaultValueIndex]) defaultValueIndex++; });

          // 初期値と条件が完全に一致しない場合は次のmemberへ
          if (memberField.DefaultValues.Count != defaultValueIndex) continue;
        }

        memberCount++;
      }

      return memberCount;
    }

  }
}
