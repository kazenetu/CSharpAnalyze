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
  [Collection("CollectionName")]
  public class ClassTest : TestBase
  {
    /// <summary>
    /// ソース作成パターン
    /// </summary>
    private enum CreatePattern
    {
      Standard,
      SubClass,
      InnerClass,
      AbstractClass,
      GenericClass
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

        case CreatePattern.AbstractClass:
          filePath = "AbstractClass.cs";

          source.AppendLine("public abstract class AbstractClass ");
          source.AppendLine("{");
          source.AppendLine("}");

          source.AppendLine("public class SubClass : AbstractClass");
          source.AppendLine("{");
          source.AppendLine("}");
          break;

        case CreatePattern.GenericClass:
          filePath = "GenericClass.cs";

          source.AppendLine("public class GenericClass<T> ");
          source.AppendLine("{");
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

           //ジェネリックの確認
           Assert.Empty(itemClass.GenericTypes);

           // スーパークラスの設定確認
           Assert.Empty(itemClass.SuperClass);

           // 親の存在確認
           Assert.Null(itemClass.Parent);

           // クラス名の確認
           Assert.Equal("ClassTest",itemClass.Name );

           // スコープ修飾子の件数確認
           Assert.Single(itemClass.Modifiers);

           // スコープ修飾子の内容確認
           Assert.Contains("public", itemClass.Modifiers);

           // ItemTypeの確認
           Assert.Equal(ItemTypes.Class, itemClass.ItemType );

           // クラス内の要素の存在確認
           Assert.Empty(itemClass.Members);
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
           Assert.Single(ev.FileRoot.OtherFiles);
           Assert.Equal("ClassTest", ev.FileRoot.OtherFiles.First().Key);
           Assert.Equal("Test.cs", ev.FileRoot.OtherFiles.First().Value);

           // 解析結果の件数確認
           Assert.Single(ev.FileRoot.Members);

           //ジェネリックの確認
           Assert.Empty(itemClass.GenericTypes);

           // スーパークラスの設定確認
           Assert.Single(itemClass.SuperClass);
           Assert.Equal("ClassTest", itemClass.SuperClass.First().Name);

           // 親の存在確認
           Assert.Null(itemClass.Parent);

           // クラス名の確認
           Assert.Equal("SubClass", itemClass.Name);

           // スコープ修飾子の件数確認
           Assert.Single(itemClass.Modifiers);

           // スコープ修飾子の内容確認
           Assert.Contains("public", itemClass.Modifiers);

           // ItemTypeの確認
           Assert.Equal(ItemTypes.Class, itemClass.ItemType);

           // クラス内の要素の存在確認
           Assert.Empty(itemClass.Members);
         });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// 抽象クラスのテスト
    /// </summary>
    [Fact(DisplayName = "AbstractClass")]
    public void AbstractClassTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.AbstractClass), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemAbstractClass = GetClassInstance(ev, "AbstractClass.cs", 0);

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        //ジェネリックの確認
        Assert.Empty(itemAbstractClass.GenericTypes);

        // スーパークラスの設定確認
        Assert.Empty(itemAbstractClass.SuperClass);

        // 親の存在確認
        Assert.Null(itemAbstractClass.Parent);

        // スコープ修飾子の件数確認
        Assert.Equal(2, itemAbstractClass.Modifiers.Count);

        // スコープ修飾子の内容確認
        Assert.Contains("public", itemAbstractClass.Modifiers);
        Assert.Contains("abstract", itemAbstractClass.Modifiers);

        // ItemTypeの確認
        Assert.Equal(ItemTypes.Class, itemAbstractClass.ItemType);

        // クラス内の要素の存在確認
        Assert.Empty(itemAbstractClass.Members);

        #region サブクラスの確認

        // サブインスタンスを取得
        var itemSubClass = GetClassInstance(ev, "AbstractClass.cs", 1);

        //ジェネリックの確認
        Assert.Empty(itemSubClass.GenericTypes);

        // スーパークラスの設定確認
        Assert.Single(itemSubClass.SuperClass);
        Assert.Equal("AbstractClass", itemSubClass.SuperClass.First().Name);

        // 親の存在確認
        Assert.Null(itemSubClass.Parent);

        // スコープ修飾子の件数確認
        Assert.Single(itemSubClass.Modifiers);

        // スコープ修飾子の内容確認
        Assert.Contains("public", itemAbstractClass.Modifiers);

        // ItemTypeの確認
        Assert.Equal(ItemTypes.Class, itemSubClass.ItemType);

        // クラス内の要素の存在確認
        Assert.Empty(itemSubClass.Members);

        #endregion
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
        Assert.Empty(ev.FileRoot.OtherFiles);

        //ジェネリックの確認
        Assert.Empty(itemClass.GenericTypes);

        //ジェネリックの確認
        Assert.Empty(itemClass.GenericTypes);

        // スーパークラスの設定確認
        Assert.Empty(itemClass.SuperClass);

        // 親の存在確認
        Assert.Null(itemClass.Parent);

        // クラス名の確認
        Assert.Equal("ClassTest", itemClass.Name);

        // スコープ修飾子の件数確認
        Assert.Single(itemClass.Modifiers);

        // スコープ修飾子の内容確認
        Assert.Contains("public", itemClass.Modifiers);

        // ItemTypeの確認
        Assert.Equal(ItemTypes.Class, itemClass.ItemType);

        // クラス内の要素の存在確認
        Assert.Single(itemClass.Members);

        #region 内部クラスの確認

        // クラス内の要素がクラス要素である確認
        Assert.IsAssignableFrom<IItemClass>(itemClass.Members[0]);
        var innerClass = itemClass.Members[0] as IItemClass;
        Assert.Equal("InnerClass", innerClass.Name);

        //ジェネリックの確認
        Assert.Empty(innerClass.GenericTypes);

        // スコープ修飾子の件数確認
        Assert.Equal(2, innerClass.Modifiers.Count);

        // スコープ修飾子の内容確認
        Assert.Contains("private", innerClass.Modifiers);
        Assert.Contains("static", innerClass.Modifiers);

        // ItemTypeの確認
        Assert.Equal(ItemTypes.Class, innerClass.ItemType);

        // クラス内の要素の存在確認
        Assert.Empty(innerClass.Members);

        #endregion
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// ジェネリッククラスのテスト
    /// </summary>
    [Fact(DisplayName = "GenericClass")]
    public void GenericClassTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.GenericClass), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "GenericClass.cs", 0);

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        //ジェネリックの確認
        Assert.Single(itemClass.GenericTypes);
        Assert.Contains("T", itemClass.GenericTypes);

        // スーパークラスの設定確認
        Assert.Empty(itemClass.SuperClass);

        // 親の存在確認
        Assert.Null(itemClass.Parent);

        // スコープ修飾子の件数確認
        Assert.Single(itemClass.Modifiers);

        // スコープ修飾子の内容確認
        Assert.Contains("public", itemClass.Modifiers);

        // ItemTypeの確認
        Assert.Equal(ItemTypes.Class, itemClass.ItemType);

        // クラス内の要素の存在確認
        Assert.Empty(itemClass.Members);
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

  }
}
