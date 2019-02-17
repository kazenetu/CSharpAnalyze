using CSharpAnalyze.Domain.Event;
using CSharpAnalyze.Domain.PublicInterfaces;
using CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems;
using CSharpAnalyze.Domain.PublicInterfaces.Events;
using CSharpAnalyzeTest.Common;
using System;
using System.Text;
using Xunit;

namespace CSharpAnalyzeTest
{
  public class ClassTest : TestBase
  {
    /// <summary>
    /// Setup
    /// </summary>
    public ClassTest():base()
    {
      System.Diagnostics.Debug.WriteLine($"Setup {Environment.CurrentDirectory}");
    }

    [Fact]
    public void Test()
    {
      // テストコード生成
      var source = new StringBuilder();
      source.AppendLine("public class ClassTest");
      source.AppendLine("{");
      source.AppendLine("}");

      // テストコードを追加
      CreateFileData("Test.cs", "", source.ToString(), (ev) =>
         {
           // ファイル名の確認
           Assert.True(ev.FilePath == "Test.cs");

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
  }

}
