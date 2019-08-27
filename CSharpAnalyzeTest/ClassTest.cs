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
      InnerClass,
      AbstractClass,
      GenericClass,
      SubAndInterface,
      SubClassMemberOverLoad,
      GenericSubClass,
      SubAndMultiInterface,
      CommentCRLF,
      CommentLF,
      InnerClassGenerics,
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
          source.AppendLine("  public class InnerClass ");
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

        case CreatePattern.SubAndInterface:
          filePath = "SubAndInterface.cs";

          source.AppendLine("public class SuperClass ");
          source.AppendLine("{");
          source.AppendLine("}");

          source.AppendLine("public interface Inf ");
          source.AppendLine("{");
          source.AppendLine("}");

          source.AppendLine("public class SubClass : SuperClass,Inf");
          source.AppendLine("{");
          source.AppendLine("}");
          break;

        case CreatePattern.SubClassMemberOverLoad:
          filePath = "SubClassMemberOverLoad.cs";

          source.AppendLine("public class SuperClass ");
          source.AppendLine("{");
          source.AppendLine("  public int FieldPublic;");
          source.AppendLine("  protected int FieldProtected;");
          source.AppendLine("  private int FieldPrivate;");

          source.AppendLine("  public int PropPublic{set;get;}");
          source.AppendLine("  protected int PropProtected{get;}");
          source.AppendLine("  private int PropPrivate{set;}");

          source.AppendLine("  public void MethodPublic(){}");
          source.AppendLine("  protected void MethodProtected(){}");
          source.AppendLine("  private void MethodPrivate(){}");
          source.AppendLine("}");

          source.AppendLine("public class SubClass : SuperClass");
          source.AppendLine("{");
          source.AppendLine("  private int FieldSubPrivate;");
          source.AppendLine("  private int PropSubPrivate{set;}");
          source.AppendLine("  private void MethodSubPrivate(){}");
          source.AppendLine("}");
          break;

        case CreatePattern.GenericSubClass:
          filePath = "GenericSubClass.cs";

          source.AppendLine("public class GenericClass<T> ");
          source.AppendLine("{");
          source.AppendLine("}");
          source.AppendLine("public class GenericSubClass<T> : GenericClass<T>");
          source.AppendLine("{");
          source.AppendLine("}");
          break;

        case CreatePattern.SubAndMultiInterface:
          filePath = "SubAndMultiInterface.cs";

          source.AppendLine("public class SuperClass ");
          source.AppendLine("{");
          source.AppendLine("}");

          source.AppendLine("public interface Inf ");
          source.AppendLine("{");
          source.AppendLine("}");

          source.AppendLine("public interface Inf2 ");
          source.AppendLine("{");
          source.AppendLine("}");

          source.AppendLine("public class SubClass : SuperClass,Inf,Inf2");
          source.AppendLine("{");
          source.AppendLine("}");
          break;

        case CreatePattern.CommentCRLF:
          filePath = "Test.cs";

          source.Append("/// <summary>\r\n");
          source.Append("/// テスト\r\n");
          source.Append("/// </summary>\r\n");
          source.AppendLine("public class ClassTest");
          source.AppendLine("{");
          source.AppendLine("}");
          break;

        case CreatePattern.CommentLF:
          filePath = "Test.cs";

          source.Append("/// <summary>\n");
          source.Append("/// テスト\n");
          source.Append("/// </summary>\n");
          source.AppendLine("public class ClassTest");
          source.AppendLine("{");
          source.AppendLine("}");
          break;

        case CreatePattern.InnerClassGenerics:
          filePath = "InnerClassGenerics.cs";

          source.AppendLine("public class GenericSubClass : GenericClass<ClassTest.InnerClass>");
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
           var expectedMethodList = new List<(List<string> modifiers, string methodName, string methodTypes, List<(string name, string expressions, string refType, string defaultValue)> expectedArgs)>();
           var expectedPropertyList = new List<(List<string> modifiers, string name, string type, Dictionary<string, List<string>> accessors, bool isInit, List<string> init)>();
           var expectedFieldList = new List<(List<string> modifiers, string name, string type, bool isInit, List<string> init)>();

           // 期待値数と一致要素数の確認
           var expectedCount = expectedMethodList.Count + expectedPropertyList.Count + expectedFieldList.Count;
           var actualCount = GetMemberCount(itemClass, expectedMethodList) + GetMemberCount(itemClass, expectedPropertyList) + GetMemberCount(itemClass, expectedFieldList);
           Assert.Equal(expectedCount, actualCount);

           // 実際の要素数との一致確認
           var actualMemberCount = itemClass.Members.Count + itemClass.BaseMethods.Count + itemClass.BaseProperties.Count + itemClass.BaseFields.Count;
           Assert.Equal(expectedCount, actualMemberCount);
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
           Assert.Equal("ClassTest", GetExpressionsToString(itemClass.SuperClass));

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
           var expectedMethodList = new List<(List<string> modifiers, string methodName, string methodTypes, List<(string name, string expressions, string refType, string defaultValue)> expectedArgs)>();
           var expectedPropertyList = new List<(List<string> modifiers, string name, string type, Dictionary<string, List<string>> accessors, bool isInit, List<string> init)>();
           var expectedFieldList = new List<(List<string> modifiers, string name, string type, bool isInit, List<string> init)>();

           // 期待値数と一致要素数の確認
           var expectedCount = expectedMethodList.Count + expectedPropertyList.Count + expectedFieldList.Count;
           var actualCount = GetMemberCount(itemClass, expectedMethodList) + GetMemberCount(itemClass, expectedPropertyList) + GetMemberCount(itemClass, expectedFieldList);
           Assert.Equal(expectedCount, actualCount);

           // 実際の要素数との一致確認
           var actualMemberCount = itemClass.Members.Count + itemClass.BaseMethods.Count + itemClass.BaseProperties.Count + itemClass.BaseFields.Count;
           Assert.Equal(expectedCount, actualMemberCount);
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

        {
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
          var expectedMethodList = new List<(List<string> modifiers, string methodName, string methodTypes, List<(string name, string expressions, string refType, string defaultValue)> expectedArgs)>();
          var expectedPropertyList = new List<(List<string> modifiers, string name, string type, Dictionary<string, List<string>> accessors, bool isInit, List<string> init)>();
          var expectedFieldList = new List<(List<string> modifiers, string name, string type, bool isInit, List<string> init)>();

          // 期待値数と一致要素数の確認
          var expectedCount = expectedMethodList.Count + expectedPropertyList.Count + expectedFieldList.Count;
          var actualCount = GetMemberCount(itemAbstractClass, expectedMethodList) + GetMemberCount(itemAbstractClass, expectedPropertyList) + GetMemberCount(itemAbstractClass, expectedFieldList);
          Assert.Equal(expectedCount, actualCount);

          // 実際の要素数との一致確認
          var actualMemberCount = itemAbstractClass.Members.Count + itemAbstractClass.BaseMethods.Count + itemAbstractClass.BaseProperties.Count + itemAbstractClass.BaseFields.Count;
          Assert.Equal(expectedCount, actualMemberCount);
        }

        #region サブクラスの確認
        {
          // サブインスタンスを取得
          var itemSubClass = GetClassInstance(ev, "AbstractClass.cs", 1);

          //ジェネリックの確認
          Assert.Empty(itemSubClass.GenericTypes);

          // スーパークラスの設定確認
          Assert.Single(itemSubClass.SuperClass);
          Assert.Equal("AbstractClass", GetExpressionsToString(itemSubClass.SuperClass));

          // 親の存在確認
          Assert.Null(itemSubClass.Parent);

          // スコープ修飾子の件数確認
          Assert.Single(itemSubClass.Modifiers);

          // スコープ修飾子の内容確認
          Assert.Contains("public", itemAbstractClass.Modifiers);

          // ItemTypeの確認
          Assert.Equal(ItemTypes.Class, itemSubClass.ItemType);

          // クラス内の要素の存在確認
          var expectedMethodList = new List<(List<string> modifiers, string methodName, string methodTypes, List<(string name, string expressions, string refType, string defaultValue)> expectedArgs)>();
          var expectedPropertyList = new List<(List<string> modifiers, string name, string type, Dictionary<string, List<string>> accessors, bool isInit, List<string> init)>();
          var expectedFieldList = new List<(List<string> modifiers, string name, string type, bool isInit, List<string> init)>();

          // 期待値数と一致要素数の確認
          var expectedCount = expectedMethodList.Count + expectedPropertyList.Count + expectedFieldList.Count;
          var actualCount = GetMemberCount(itemSubClass, expectedMethodList) + GetMemberCount(itemSubClass, expectedPropertyList) + GetMemberCount(itemSubClass, expectedFieldList);
          Assert.Equal(expectedCount, actualCount);

          // 実際の要素数との一致確認
          var actualMemberCount = itemSubClass.Members.Count + itemSubClass.BaseMethods.Count + itemSubClass.BaseProperties.Count + itemSubClass.BaseFields.Count;
          Assert.Equal(expectedCount, actualMemberCount);
        }
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
        Assert.Single(innerClass.Modifiers);

        // スコープ修飾子の内容確認
        Assert.Contains("public", innerClass.Modifiers);

        // ItemTypeの確認
        Assert.Equal(ItemTypes.Class, innerClass.ItemType);

        // クラス内の要素の存在確認
        Assert.Empty(innerClass.Members);

        // クラス内の要素の存在確認
        var expectedMethodList = new List<(List<string> modifiers, string methodName, string methodTypes, List<(string name, string expressions, string refType, string defaultValue)> expectedArgs)>();
        var expectedPropertyList = new List<(List<string> modifiers, string name, string type, Dictionary<string, List<string>> accessors, bool isInit, List<string> init)>();
        var expectedFieldList = new List<(List<string> modifiers, string name, string type, bool isInit, List<string> init)>();

        // 期待値数と一致要素数の確認
        var expectedCount = expectedMethodList.Count + expectedPropertyList.Count + expectedFieldList.Count;
        var actualCount = GetMemberCount(innerClass, expectedMethodList) + GetMemberCount(innerClass, expectedPropertyList) + GetMemberCount(innerClass, expectedFieldList);
        Assert.Equal(expectedCount, actualCount);

        // 実際の要素数との一致確認
        var actualMemberCount = innerClass.Members.Count + innerClass.BaseMethods.Count + innerClass.BaseProperties.Count + innerClass.BaseFields.Count;
        Assert.Equal(expectedCount, actualMemberCount);

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
        var expectedMethodList = new List<(List<string> modifiers, string methodName, string methodTypes, List<(string name, string expressions, string refType, string defaultValue)> expectedArgs)>();
        var expectedPropertyList = new List<(List<string> modifiers, string name, string type, Dictionary<string, List<string>> accessors, bool isInit, List<string> init)>();
        var expectedFieldList = new List<(List<string> modifiers, string name, string type, bool isInit, List<string> init)>();

        // 期待値数と一致要素数の確認
        var expectedCount = expectedMethodList.Count + expectedPropertyList.Count + expectedFieldList.Count;
        var actualCount = GetMemberCount(itemClass, expectedMethodList) + GetMemberCount(itemClass, expectedPropertyList) + GetMemberCount(itemClass, expectedFieldList);
        Assert.Equal(expectedCount, actualCount);

        // 実際の要素数との一致確認
        var actualMemberCount = itemClass.Members.Count + itemClass.BaseMethods.Count + itemClass.BaseProperties.Count + itemClass.BaseFields.Count;
        Assert.Equal(expectedCount, actualMemberCount);
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// スーパークラス+インターフェースのテスト
    /// </summary>
    [Fact(DisplayName = "SubAndInterface")]
    public void SubAndInterface()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.SubAndInterface), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "SubAndInterface.cs", 2);

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 解析結果の件数確認
        Assert.True(ev.FileRoot.Members.Count == 3);

        //ジェネリックの確認
        Assert.Empty(itemClass.GenericTypes);

        // スーパークラスの設定確認
        Assert.Single(itemClass.SuperClass);
        Assert.Equal("SuperClass", GetExpressionsToString(itemClass.SuperClass));

        // インターフェースの設定確認
        Assert.Single(itemClass.Interfaces);
        Assert.Equal("Inf", GetExpressionsToString(itemClass.Interfaces.First()));

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
        var expectedMethodList = new List<(List<string> modifiers, string methodName, string methodTypes, List<(string name, string expressions, string refType, string defaultValue)> expectedArgs)>();
        var expectedPropertyList = new List<(List<string> modifiers, string name, string type, Dictionary<string, List<string>> accessors, bool isInit, List<string> init)>();
        var expectedFieldList = new List<(List<string> modifiers, string name, string type, bool isInit, List<string> init)>();

        // 期待値数と一致要素数の確認
        var expectedCount = expectedMethodList.Count + expectedPropertyList.Count + expectedFieldList.Count;
        var actualCount = GetMemberCount(itemClass, expectedMethodList) + GetMemberCount(itemClass, expectedPropertyList) + GetMemberCount(itemClass, expectedFieldList);
        Assert.Equal(expectedCount, actualCount);

        // 実際の要素数との一致確認
        var actualMemberCount = itemClass.Members.Count + itemClass.BaseMethods.Count + itemClass.BaseProperties.Count + itemClass.BaseFields.Count;
        Assert.Equal(expectedCount, actualMemberCount);

      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// スーパークラスのメンバ―のテスト
    /// </summary>
    [Fact(DisplayName = "SubClassMemberOverLoad")]
    public void SubClassMemberOverLoadTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.SubClassMemberOverLoad), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "SubClassMemberOverLoad.cs", 1);

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 解析結果の件数確認
        Assert.True(ev.FileRoot.Members.Count == 2);

        //ジェネリックの確認
        Assert.Empty(itemClass.GenericTypes);

        // スーパークラスの設定確認
        Assert.Single(itemClass.SuperClass);
        Assert.Equal("SuperClass", GetExpressionsToString(itemClass.SuperClass));

        // インターフェースの設定確認
        Assert.Empty(itemClass.Interfaces);

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
        var expectedMethodList = new List<(List<string> modifiers, string methodName, string methodTypes, List<(string name, string expressions, string refType, string defaultValue)> expectedArgs)>
        {
          (new List<string>{"public" }, "MethodPublic","void",
            new List<(string name, string expressions, string refType, string defaultValue)>
            {
            }
          ),
          (new List<string>{"protected" }, "MethodProtected","void",
            new List<(string name, string expressions, string refType, string defaultValue)>
            {
            }
          ),
          // スーパークラスのprivateスコープは継承対象外
          //(new List<string>{"private" }, "MethodPrivate","void",
          //  new List<(string name, string expressions, string refType, string defaultValue)>
          //  {
          //  }
          //),
          (new List<string>{"private" }, "MethodSubPrivate","void",
            new List<(string name, string expressions, string refType, string defaultValue)>
            {
            }
          ),
        };
        var expectedPropertyList = new List<(List<string> modifiers, string name, string type, Dictionary<string, List<string>> accessors, bool isInit, List<string> init)>
        {
            (new List<string>{"public" },"PropPublic", "int", new Dictionary<string, List<string>>() { { "set", new List<string>() }, { "get", new List<string>() }}, false, null),
            (new List<string>{"protected" },"PropProtected", "int", new Dictionary<string, List<string>>() { { "get", new List<string>() } }, true, new List<string>{ "1"}),
            // スーパークラスのprivateスコープは継承対象外
            //(new List<string>{"private " },"PropPrivate", "int", new Dictionary<string, List<string>>() { { "set", new List<string>() } }, false, null),
            (new List<string>{"private" },"PropSubPrivate", "int", new Dictionary<string, List<string>>() { { "set", new List<string>() } }, false, null),
        };
        var expectedFieldList = new List<(List<string> modifiers, string name, string type, bool isInit, List<string> init)>
        {
          (new List<string>() { "public" }, "FieldPublic", "int", false, null),
          (new List<string>() { "protected" }, "FieldProtected", "int", false, null),
          // スーパークラスのprivateスコープは継承対象外
          //(new List<string>() { "private" }, "FieldPrivate", "int", false, null),
          (new List<string>() { "private" }, "FieldSubPrivate", "int", false, null),
        };

        // 期待値数と一致要素数の確認
        var expectedCount = expectedMethodList.Count + expectedPropertyList.Count+ expectedFieldList.Count;
        var actualCount = GetMemberCount(itemClass, expectedMethodList) + GetMemberCount(itemClass, expectedPropertyList) + GetMemberCount(itemClass, expectedFieldList);
        Assert.Equal(expectedCount, actualCount);

        // 実際の要素数との一致確認
        var actualMemberCount = itemClass.Members.Count + itemClass.BaseMethods.Count + itemClass.BaseProperties.Count + itemClass.BaseFields.Count;
        Assert.Equal(expectedCount, actualMemberCount);
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// ジェネリッククラス サブクラスのテスト
    /// </summary>
    [Fact(DisplayName = "GenericSubClass")]
    public void GenericSubClassTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.GenericSubClass), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "GenericSubClass.cs", 1);

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        //ジェネリックの確認
        Assert.Single(itemClass.GenericTypes);
        Assert.Contains("T", itemClass.GenericTypes);

        // スーパークラスの設定確認
        var expectedSuperClass = new List<string>(){
          "GenericClass",
          "<",
          "T",
          ">",
        };
        Assert.Equal(expectedSuperClass, itemClass.SuperClass.Select(item=>item.Name));


        // 親の存在確認
        Assert.Null(itemClass.Parent);

        // スコープ修飾子の件数確認
        Assert.Single(itemClass.Modifiers);

        // スコープ修飾子の内容確認
        Assert.Contains("public", itemClass.Modifiers);

        // ItemTypeの確認
        Assert.Equal(ItemTypes.Class, itemClass.ItemType);

        // クラス内の要素の存在確認
        var expectedMethodList = new List<(List<string> modifiers, string methodName, string methodTypes, List<(string name, string expressions, string refType, string defaultValue)> expectedArgs)>();
        var expectedPropertyList = new List<(List<string> modifiers, string name, string type, Dictionary<string, List<string>> accessors, bool isInit, List<string> init)>();
        var expectedFieldList = new List<(List<string> modifiers, string name, string type, bool isInit, List<string> init)>();

        // 期待値数と一致要素数の確認
        var expectedCount = expectedMethodList.Count + expectedPropertyList.Count + expectedFieldList.Count;
        var actualCount = GetMemberCount(itemClass, expectedMethodList) + GetMemberCount(itemClass, expectedPropertyList) + GetMemberCount(itemClass, expectedFieldList);
        Assert.Equal(expectedCount, actualCount);

        // 実際の要素数との一致確認
        var actualMemberCount = itemClass.Members.Count + itemClass.BaseMethods.Count + itemClass.BaseProperties.Count + itemClass.BaseFields.Count;
        Assert.Equal(expectedCount, actualMemberCount);
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// スーパークラス+インターフェースのテスト
    /// </summary>
    [Fact(DisplayName = "SubAndMultiInterfaceTest")]
    public void SubAndMultiInterfaceTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.SubAndMultiInterface), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "SubAndMultiInterface.cs", 3);

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 解析結果の件数確認
        Assert.True(ev.FileRoot.Members.Count == 4);

        //ジェネリックの確認
        Assert.Empty(itemClass.GenericTypes);

        // スーパークラスの設定確認
        Assert.Single(itemClass.SuperClass);
        Assert.Equal("SuperClass", GetExpressionsToString(itemClass.SuperClass));

        // インターフェースの設定確認
        Assert.True(itemClass.Interfaces.Count == 2);
        Assert.Equal("Inf", GetExpressionsToString(itemClass.Interfaces.First()));
        Assert.Equal("Inf2", GetExpressionsToString(itemClass.Interfaces.Last()));

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
        var expectedMethodList = new List<(List<string> modifiers, string methodName, string methodTypes, List<(string name, string expressions, string refType, string defaultValue)> expectedArgs)>();
        var expectedPropertyList = new List<(List<string> modifiers, string name, string type, Dictionary<string, List<string>> accessors, bool isInit, List<string> init)>();
        var expectedFieldList = new List<(List<string> modifiers, string name, string type, bool isInit, List<string> init)>();

        // 期待値数と一致要素数の確認
        var expectedCount = expectedMethodList.Count + expectedPropertyList.Count + expectedFieldList.Count;
        var actualCount = GetMemberCount(itemClass, expectedMethodList) + GetMemberCount(itemClass, expectedPropertyList) + GetMemberCount(itemClass, expectedFieldList);
        Assert.Equal(expectedCount, actualCount);

        // 実際の要素数との一致確認
        var actualMemberCount = itemClass.Members.Count + itemClass.BaseMethods.Count + itemClass.BaseProperties.Count + itemClass.BaseFields.Count;
        Assert.Equal(expectedCount, actualMemberCount);

      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// コメント改行テスト：CRLF
    /// </summary>
    [Fact(DisplayName = "CommentCRLF")]
    public void CommentCRLFTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.CommentCRLF), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "Test.cs", 0);

        // コメント数の確認
        Assert.Equal(3, itemClass.Comments.Count);
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// コメント改行テスト：LF
    /// </summary>
    [Fact(DisplayName = "CommentLF")]
    public void CommentLFTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.CommentLF), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "Test.cs", 0);

        // コメント数の確認
        Assert.Equal(3, itemClass.Comments.Count);
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// 内部クラスを含むジェネリッククラスのテスト
    /// </summary>
    [Fact(DisplayName = "InnerClassGenerics")]
    public void InnerClassGenericsTest()
    {
      // 内部クラス用テストコードを追加
      CreateFileData(CreateSource(CreatePattern.InnerClass), null);

      // スーパークラス用テストコードを追加
      CreateFileData(CreateSource(CreatePattern.GenericClass), null);

      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.InnerClassGenerics), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "InnerClassGenerics.cs", 0);

        // 外部参照の存在確認
        Assert.Equal(2,ev.FileRoot.OtherFiles.Count());

        //ジェネリックの確認
        Assert.Empty(itemClass.GenericTypes);

        // スーパークラスの設定確認
        var expectedSuperClass = new List<string>(){
          "GenericClass",
          "<",
          "ClassTest.InnerClass",
          ">",
        };
        Assert.Equal(expectedSuperClass, itemClass.SuperClass.Select(item => item.Name));

        // スコープ修飾子の件数確認
        Assert.Single(itemClass.Modifiers);

        // スコープ修飾子の内容確認
        Assert.Contains("public", itemClass.Modifiers);

        // ItemTypeの確認
        Assert.Equal(ItemTypes.Class, itemClass.ItemType);

        // クラス内の要素の存在確認
        var expectedMethodList = new List<(List<string> modifiers, string methodName, string methodTypes, List<(string name, string expressions, string refType, string defaultValue)> expectedArgs)>();
        var expectedPropertyList = new List<(List<string> modifiers, string name, string type, Dictionary<string, List<string>> accessors, bool isInit, List<string> init)>();
        var expectedFieldList = new List<(List<string> modifiers, string name, string type, bool isInit, List<string> init)>();

        // 期待値数と一致要素数の確認
        var expectedCount = expectedMethodList.Count + expectedPropertyList.Count + expectedFieldList.Count;
        var actualCount = GetMemberCount(itemClass, expectedMethodList) + GetMemberCount(itemClass, expectedPropertyList) + GetMemberCount(itemClass, expectedFieldList);
        Assert.Equal(expectedCount, actualCount);

        // 実際の要素数との一致確認
        var actualMemberCount = itemClass.Members.Count + itemClass.BaseMethods.Count + itemClass.BaseProperties.Count + itemClass.BaseFields.Count;
        Assert.Equal(expectedCount, actualMemberCount);
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    #region メンバー数取得
    /// <summary>
    /// メンバー数を取得:メソッド用
    /// </summary>
    /// <param name="targetInstance">対象のインスタンス</param>
    /// <param name="expectedList">メソッド名とパラメータの期待値</param>
    /// <returns>条件が一致するメンバー数</returns>
    private int GetMemberCount(IItemClass targetInstance, List<(List<string> modifiers, string methodName, string methodTypes, List<(string name, string expressions, string refType, string defaultValue)> expectedArgs)> expectedList)
    {
      var memberCount = 0;
      foreach (var member in targetInstance.Members)
      {
        // 対象以外は次のmemberへ
        if (!(member is IItemMethod memberMethod)) continue;

        // 予想リストから同じ名前のメソッドを取得する
        var expectedMethods = expectedList.Where(item => item.methodName == member.Name);
        if (!expectedMethods.Any()) continue;

        // 予想リスト結果から対象が存在するか確認する
        foreach (var expectedMethod in expectedMethods)
        {
          // アクセス修飾子の確認
          if (string.Join(" ", expectedMethod.modifiers.OrderBy(item=>item)) == string.Join(" ", memberMethod.Modifiers.OrderBy(item => item)))
            continue;

          // 型タイプの確認
          if (expectedMethod.methodTypes != GetExpressionsToString(memberMethod.MethodTypes))
            continue;

          // パラメータ取得
          var expectedArgs = expectedMethod.expectedArgs;

          // パラメータ数の確認
          if (expectedArgs.Count != memberMethod.Args.Count) continue;

          // パラメータの確認
          var argCount = 0;
          foreach (var (name, expressions, refType, defaultValue) in expectedArgs)
          {
            var actualArgs = memberMethod.Args
                            .Where(arg => arg.name == name)
                            .Where(arg => GetExpressionsToString(arg.expressions) == expressions)
                            .Where(arg => string.Concat(arg.modifiers) == refType)
                            .Where(arg => GetExpressionsToString(arg.defaultValues) == defaultValue);
            if (actualArgs.Any())
            {
              argCount++;
            }
          }

          // 一致パラメータ数の確認
          Assert.Equal(expectedArgs.Count, argCount);
        }

        memberCount++;
      }

      // 参考情報：継承元のメソッドを確認
      foreach (var baseMethod in targetInstance.BaseMethods)
      {
        // 予想リストから同じ名前のメソッドを取得する
        var expectedMethods = expectedList.Where(item => baseMethod.StartsWith($"{string.Join(" ", item.modifiers)} {item.methodTypes} {item.methodName}(", StringComparison.CurrentCulture));
        if (!expectedMethods.Any()) continue;

        foreach (var expectedMethod in expectedMethods)
        {
          var expectedResult = new StringBuilder();
          expectedResult.Append($"{string.Join(" ", expectedMethod.modifiers)} {expectedMethod.methodTypes} {expectedMethod.methodName}(");

          // パラメータ取得
          var expectedArgs = expectedMethod.expectedArgs;

          // 文字列にパラメータを追加
          var isFirst = true;
          foreach (var (name, expressions, refType, defaultValue) in expectedArgs)
          {
            if (!isFirst)
            {
              expectedResult.Append(", ");
            }
            expectedResult.Append($"{expressions} {name}");

            // デフォルト値がある場合は設定
            if (!string.IsNullOrEmpty(defaultValue))
            {
              expectedResult.Append($" = {defaultValue}");
            }

            isFirst = false;
          }
          expectedResult.Append(")");

          // 参考情報と一致確認
          if (expectedResult.ToString() == baseMethod)
          {
            memberCount++;
          }
        }
      }

      return memberCount;
    }

    /// <summary>
    /// メンバー数を取得：プロパティ
    /// </summary>
    /// <param name="targetInstance">対象のインスタンス</param>
    /// <param name="expectedList">予想値リスト</param>
    /// <returns>条件が一致するメンバー数</returns>
    private int GetMemberCount(IItemClass itemClass, List<(List<string> modifiers, string name, string type, Dictionary<string, List<string>> accessors, bool isInit, List<string> init)> expectedList)
    {
      var memberCount = 0;
      foreach (var member in itemClass.Members)
      {
        // 対象以外は次のmemberへ
        if (!(member is IItemProperty memberProperty)) continue;

        // 型の一致確認
        var targetProperties = expectedList.Where(field => field.name == memberProperty.Name && field.type == GetExpressionsToString(memberProperty.PropertyTypes));
        if (!targetProperties.Any()) continue;

        // 条件取得
        var (modifiers, name, type, accessors, isInit, init) = targetProperties.First();

        // アクセサの一致確認
        var accessorCount = 0;
        foreach (var expectedItem in accessors)
        {
          // 対象のアクセサの存在確認
          var targets = memberProperty.AccessorList.Where(accessor => accessor.Name == expectedItem.Key);
          Assert.Single(targets);

          // 対象を取得
          var target = targets.First();

          // コード数の一致確認
          Assert.Equal(expectedItem.Value.Count, target.Members.Count);

          // コードの確認
          var accessorMemberCount = 0;
          foreach (var expectedMember in expectedItem.Value)
          {
            Assert.Equal(expectedMember, target.Members[accessorMemberCount].ToString());
            accessorMemberCount++;
          }

          accessorCount++;
        }
        // 確認済アクセサ数の一致確認
        Assert.Equal(accessorCount, memberProperty.AccessorList.Count);

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
        else
        {
          Assert.Empty(memberProperty.DefaultValues);
        }

        memberCount++;
      }

      // 参考情報：継承元を確認
      foreach (var baseProperty in itemClass.BaseProperties)
      {
        // 予想リストから同じ名前のプロパティを取得する
        var targetProperties = expectedList.Where(item => baseProperty.StartsWith($"{string.Join(" " ,item.modifiers)} {item.type} {item.name}", StringComparison.CurrentCulture));
        if (!targetProperties.Any()) continue;

        foreach (var expectedProperty in targetProperties)
        {
          var expectedResult = new StringBuilder();
          expectedResult.Append($"{string.Join(" ", expectedProperty.modifiers)} {expectedProperty.type} {expectedProperty.name}");

          if (expectedProperty.accessors.Any())
          {
            expectedResult.Append("{");
            // 文字列にアクセサを追加
            foreach (var expectedItem in expectedProperty.accessors)
            {
              expectedResult.Append($"{expectedItem.Key};");
            }
            expectedResult.Append("}");
          }
          else
          {
            // アクセサがない場合はセミコロンを追加
            expectedResult.Append(";");
          }

          // 参考情報と一致確認
          if (expectedResult.ToString() == baseProperty)
          {
            memberCount++;
          }
        }
      }

      return memberCount;
    }

    /// <summary>
    /// メンバー数を取得：フィールド
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

        // 型の一致確認
        var targetFileds = expectedList.Where(field => field.name == memberField.Name && field.type == GetExpressionsToString(memberField.FieldTypes));
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
        else
        {
          Assert.Empty(memberField.DefaultValues);
        }

        memberCount++;
      }

      // 参考情報：継承元を確認
      foreach (var baseField in itemClass.BaseFields)
      {
        // 予想リストから同じ名前のプロパティを取得する
        var targetFilds = expectedList.Where(item => baseField.StartsWith($"{string.Join(" ", item.modifiers)} {item.type} {item.name}", StringComparison.CurrentCulture));
        if (!targetFilds.Any()) continue;

        foreach (var expectedField in targetFilds)
        {
          var expectedResult = new StringBuilder();
          expectedResult.Append($"{string.Join(" ", expectedField.modifiers)} {expectedField.type} {expectedField.name};");

          // 参考情報と一致確認
          if (expectedResult.ToString() == baseField)
          {
            memberCount++;
          }
        }
      }

      return memberCount;
    }

    #endregion 
  }
}
