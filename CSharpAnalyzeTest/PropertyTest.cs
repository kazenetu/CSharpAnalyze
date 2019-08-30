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
      RefField,
      Static,
      ClassProperty,
      Generic,
      LamdaGet,
      LamdaSetGet,
      TempInnerClass,
      InnerClassProperty,
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

        case CreatePattern.RefField:
          filePath = "RefField.cs";

          source.AppendLine("public class RefFieldTest");
          source.AppendLine("{");
          source.AppendLine("  private string fieldString;");
          source.AppendLine("  public string PropertyString");
          source.AppendLine("  {");
          source.AppendLine("    set{");
          source.AppendLine("      fieldString = value;");
          source.AppendLine("    }");
          source.AppendLine("    get{");
          source.AppendLine("      return fieldString;");
          source.AppendLine("    }");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;

        case CreatePattern.Static:
          filePath = "Static.cs";

          source.AppendLine("public class StaticTest");
          source.AppendLine("{");
          source.AppendLine("  public static string PropertyString{set; get;}");
          source.AppendLine("}");
          break;

        case CreatePattern.ClassProperty:
          filePath = "ClassProperty.cs";

          source.AppendLine("public class ClassPropertyTest");
          source.AppendLine("{");
          source.AppendLine("  public ClassTest PropertyString{set; get;}");
          source.AppendLine("}");
          break;

        case CreatePattern.Generic:
          filePath = "Generic.cs";

          source.AppendLine("public class GenericTest");
          source.AppendLine("{");
          source.AppendLine("  public List<string> PropertyList{set; get;} = new List<string>();");
          source.AppendLine("}");
          break;

        case CreatePattern.LamdaGet:
          filePath = "LamdaGet.cs";

          source.AppendLine("public class GenericTest");
          source.AppendLine("{");
          source.AppendLine("  private string s;");
          source.AppendLine("  public string S => s; ");
          source.AppendLine("}");
          break;

        case CreatePattern.LamdaSetGet:
          filePath = "LamdaSetGet.cs";

          source.AppendLine("public class GenericTest");
          source.AppendLine("{");
          source.AppendLine("  private string s;");
          source.AppendLine("  public string S { get => s; set => s = value; }");
          source.AppendLine("}");
          break;

        case CreatePattern.TempInnerClass:
          filePath = "TempInnerClass.cs";

          source.AppendLine("public class TempInnerClass");
          source.AppendLine("{");
          source.AppendLine("  public class InnerClass");
          source.AppendLine("  {");
          source.AppendLine("  }");
          source.AppendLine("}");
          break;

        case CreatePattern.InnerClassProperty:
          filePath = "InnerClassProperty.cs";

          source.AppendLine("public class InnerClassField");
          source.AppendLine("{");
          source.AppendLine("  public TempInnerClass.InnerClass Property1 {get;} = new TempInnerClass.InnerClass()");
          source.AppendLine("}");
          break;
      }

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
        var expectedList = new List<(List<string> modifiers, string name, string type, Dictionary<string, List<string>> accessors, bool isInit, List<string> init)>
           {
             (new List<string>() { "public" }, "PropertyString", "string",new Dictionary<string,List<string>>(){ { "set",new List<string>()  },{ "get", new List<string>() } } , false, null),
             (new List<string>() { "public" }, "PropertydInt", "int",new Dictionary<string,List<string>>(){{ "get", new List<string>() } }, true, new List<string>() { "1" }),
           };
        Assert.Equal(expectedList.Count, GetMemberCount(itemClass, expectedList));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// プライベートフィールドのアクセスプロパティのテスト
    /// </summary>
    [Fact(DisplayName = "RefField")]
    public void RefFieldTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.RefField), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "RefField.cs");

        // クラス内の要素の存在確認
        var expectedList = new List<(List<string> modifiers, string name, string type, Dictionary<string, List<string>> accessors, bool isInit, List<string> init)>
           {
             (new List<string>() { "public" }, "PropertyString", "string",new Dictionary<string,List<string>>(){ { "set",new List<string>() { "this.fieldString = value;" } },{ "get", new List<string>() { "return this.fieldString;" } } } , false, null),
           };
        Assert.Equal(expectedList.Count, GetMemberCount(itemClass, expectedList));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// クラスプロパティのテスト
    /// </summary>
    [Fact(DisplayName = "Static")]
    public void StaticTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.Static), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "Static.cs");

        // クラス内の要素の存在確認
        var expectedList = new List<(List<string> modifiers, string name, string type, Dictionary<string, List<string>> accessors, bool isInit, List<string> init)>
           {
             (new List<string>() { "public","static" }, "PropertyString", "string",new Dictionary<string,List<string>>(){ { "set",new List<string>() },{ "get", new List<string>() } } , false, null),
           };
        Assert.Equal(expectedList.Count, GetMemberCount(itemClass, expectedList));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// クラス型プロパティのテスト
    /// </summary>
    [Fact(DisplayName = "ClassProperty")]
    public void ClassPropertyTest()
    {
      // スーパークラスを追加
      CreateFileData(CreateSource(CreatePattern.Standard), null);

      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.ClassProperty), (ev) =>
      {
        // 外部参照の存在確認
        Assert.Single(ev.FileRoot.OtherFiles);
        Assert.Equal("ClassTest", ev.FileRoot.OtherFiles.First().Key);
        Assert.Equal("Standard.cs", ev.FileRoot.OtherFiles.First().Value);

        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "ClassProperty.cs");

        // クラス内の要素の存在確認
        var expectedList = new List<(List<string> modifiers, string name, string type, Dictionary<string, List<string>> accessors, bool isInit, List<string> init)>
           {
             (new List<string>() { "public" }, "PropertyString", "ClassTest",new Dictionary<string,List<string>>(){ { "set",new List<string>()  },{ "get", new List<string>() } } , false, null),
           };
        Assert.Equal(expectedList.Count, GetMemberCount(itemClass, expectedList));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// 組み込みジェネリッククラス型プロパティのテスト
    /// </summary>
    [Fact(DisplayName = "Generic")]
    public void GenericTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.Generic), (ev) =>
      {
        // 外部参照の存在確認
        Assert.Single(ev.FileRoot.OtherFiles);
        Assert.Equal("List", ev.FileRoot.OtherFiles.First().Key);
        Assert.Equal(string.Empty, ev.FileRoot.OtherFiles.First().Value);

        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "Generic.cs");

        // クラス内の要素の存在確認
        var expectedList = new List<(List<string> modifiers, string name, string type, Dictionary<string, List<string>> accessors, bool isInit, List<string> init)>
           {
             (new List<string>() { "public" }, "PropertyList", "List<string>",new Dictionary<string,List<string>>(){ { "set",new List<string>()  },{ "get", new List<string>() } } , true, new List<string>() { "new", "List","<","string",">", "(", ")" }),
           };
        Assert.Equal(expectedList.Count, GetMemberCount(itemClass, expectedList));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }


    /// <summary>
    /// 式形式のテスト:Get
    /// </summary>
    [Fact(DisplayName = "LamdaGet")]
    public void LamdaGetTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.LamdaGet), (ev) =>
      {
        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "LamdaGet.cs");

        // クラス内の要素の存在確認
        var expectedList = new List<(List<string> modifiers, string name, string type, Dictionary<string, List<string>> accessors, bool isInit, List<string> init)>
           {
             (new List<string>() { "public" }, "S", "string",new Dictionary<string,List<string>>(){ { "get", new List<string>() { "return this.s;" } } } , true, new List<string>()),
           };
        Assert.Equal(expectedList.Count, GetMemberCount(itemClass, expectedList));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// 式形式のテスト:SetGet
    /// </summary>
    [Fact(DisplayName = "LamdaSetGet")]
    public void LamdaSetGetTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.LamdaSetGet), (ev) =>
      {
        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "LamdaSetGet.cs");

        // クラス内の要素の存在確認
        var expectedList = new List<(List<string> modifiers, string name, string type, Dictionary<string, List<string>> accessors, bool isInit, List<string> init)>
           {
             (new List<string>() { "public" }, "S", "string",new Dictionary<string,List<string>>(){ { "set",new List<string>() { "this.s = value;" } },{ "get", new List<string>() { "return this.s;" } } } , false, null),
           };
        Assert.Equal(expectedList.Count, GetMemberCount(itemClass, expectedList));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// 内部クラス型プロパティのテスト
    /// </summary>
    [Fact(DisplayName = "InnerClassProperty")]
    public void InnerClassPropertyTest()
    {
      // 内部クラステストコードを追加
      CreateFileData(CreateSource(CreatePattern.TempInnerClass), null);

      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.InnerClassProperty), (ev) =>
      {
        // 外部参照の存在確認
        Assert.Single(ev.FileRoot.OtherFiles);
        Assert.Equal("TempInnerClass.InnerClass", ev.FileRoot.OtherFiles.First().Key);
        Assert.Equal("TempInnerClass.cs", ev.FileRoot.OtherFiles.First().Value);

        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "InnerClassProperty.cs");

        // クラス内の要素の存在確認
        var expectedList = new List<(List<string> modifiers, string name, string type, Dictionary<string, List<string>> accessors, bool isInit, List<string> init)>
           {
             (new List<string>() { "public" }, "Property1", "TempInnerClass.InnerClass",new Dictionary<string,List<string>>(){{ "get", new List<string>() } } , true, new List<string>() { "new", "TempInnerClass.InnerClass", "(", ")" }),
           };
        Assert.Equal(expectedList.Count, GetMemberCount(itemClass, expectedList));
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    #region ユーティリティメソッド
    /// <summary>
    /// メンバー数を取得
    /// </summary>
    /// <param name="itemClass">対象のアイテムクラス</param>
    /// <param name="expectedList">予想値リスト</param>
    /// <returns>条件が一致するメンバー数</returns>
    private int GetMemberCount(IItemClass itemClass, List<(List<string> modifiers, string name, string type, Dictionary<string,List<string>> accessors, bool isInit, List<string> init)> expectedList)
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
        foreach(var expectedItem in accessors)
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
          foreach(var expectedMember in expectedItem.Value)
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

      return memberCount;
    }
    #endregion

  }
}
