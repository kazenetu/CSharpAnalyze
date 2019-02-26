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
           Assert.True(itemClass.Members.Count == 2);

           var fields = new List<(string name, string type, bool isInit, object init)>();
           fields.Add(("fieldString", "string", false, null));
           fields.Add(("fieldInt", "int", true, "1"));

           var memberCount = 0;
           foreach (var member in itemClass.Members)
           {
             var memberField = member as IItemField;
             if (memberField == null) continue;

             var memberFieldType = new StringBuilder();
             memberField.FieldTypes.ForEach(item => memberFieldType.Append(item.Name));
             var targetFileds = fields.Where(field => field.name == memberField.Name && field.type == memberFieldType.ToString());
             if (!targetFileds.Any())
             {
               continue;
             }

             var targetFiled = targetFileds.First();
             if (targetFiled.isInit)
             {
               if (memberField.DefaultValues.Count != 1) continue;
               if (memberField.DefaultValues[0].Name != targetFiled.init.ToString()) continue;
             }

             memberCount++;
           }

           Assert.True(itemClass.Members.Count == memberCount);
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
           Assert.True(itemClass.Members.Count == 3);

           var fields = new List<(string name, string type, bool isInit, object init)>();
           fields.Add(("fieldClass1", "ClassTest", false, null));
           fields.Add(("fieldClass2", "ClassTest", true, "new ClassTest()"));
           fields.Add(("fieldClass3", "ClassTest", true, "null"));

           var memberCount = 0;
           foreach (var member in itemClass.Members)
           {
             var memberField = member as IItemField;
             if (memberField == null) continue;

             var memberFieldType = new StringBuilder();
             memberField.FieldTypes.ForEach(item => memberFieldType.Append(item.Name));
             var targetFileds = fields.Where(field => field.name == memberField.Name && field.type == memberFieldType.ToString());
             if (!targetFileds.Any())
             {
               continue;
             }

             var targetFiled = targetFileds.First();
             if (targetFiled.isInit)
             {
               if (memberField.DefaultValues.Count != 1) continue;
               if (memberField.DefaultValues[0].Name != targetFiled.init.ToString()) continue;
             }

             memberCount++;
           }

           Assert.True(itemClass.Members.Count == memberCount);
         });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }


  }
}
