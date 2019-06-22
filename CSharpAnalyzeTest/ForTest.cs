using CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems;
using CSharpAnalyzeTest.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CSharpAnalyzeTest
{
  [Trait("Forのテスト", nameof(ForTest))]
  public class ForTest : TestBase
  {
    /// <summary>
    /// ソース作成パターン
    /// </summary>
    private enum CreatePattern
    {
      Standard,
      LocalField,
      MultiDeclarations,
      MultiIncrementors,
      UseMethods,
      InstanceProperty,
      InstanceMethod,
    }

    /// <summary>
    /// ファイル名、ソースコード取得処理
    /// </summary>
    /// <param name="pattern">生成パターン</param>
    /// <returns>ファイルパスとソースコード</returns>
    private FileData CreateSource(CreatePattern pattern)
    {
      var usingList = new StringBuilder();
      var source = new List<string>();
      var filePath = string.Empty;
      var addMember = new List<string>();
      var addSource = new StringBuilder();

      switch (pattern)
      {
        case CreatePattern.Standard:
          filePath = "Standard.cs";

          source.Add("for(var index = 0;index < 10;index++)");
          source.Add("{");
          source.Add("}");
          break;

        case CreatePattern.LocalField:
          filePath = "LocalField.cs";

          source.Add("var index;");
          source.Add("for(index = 0;index < 10;index++)");
          source.Add("{");
          source.Add("}");
          break;

        case CreatePattern.MultiDeclarations:
          filePath = "MultiDeclarations.cs";

          source.Add("for(int a=0,b=0;a<10;a++,b++)");
          source.Add("{");
          source.Add("}");
          break;

        case CreatePattern.MultiIncrementors:
          filePath = "MultiIncrementors.cs";

          source.Add("var b=1D;");
          source.Add("for(int a=0;a<10;a++,b+=1D)");
          source.Add("{");
          source.Add("}");
          break;

        case CreatePattern.UseMethods:
          filePath = "UseMethods.cs";

          source.Add("for(var index = ini();condition(index);index+=incrementor())");
          source.Add("{");
          source.Add("}");
          source.Add("int ini()=>0;");
          source.Add("bool condition(int value)=> value < 10;");
          source.Add("int incrementor()=>1;");
          break;

        case CreatePattern.InstanceProperty:
          filePath = "InstanceProperty.cs";

          addMember.Add("private List<int> Values{get;}= new List<int>();");

          source.Add("Values.AddRange(new List<int>{0,1,2,3});");
          source.Add("for(var index = 0;index < Values.Count;index++)");
          source.Add("{");
          source.Add("}");
          break;

        case CreatePattern.InstanceMethod:
          filePath = "InstanceMethod.cs";

          addMember.Add("private List<int> GetValues()");
          addMember.Add("{");
          addMember.Add("  return new List<int>{0,1,2,3};");
          addMember.Add("{");

          source.Add("for(var index = 0;index < GetValues().Count;index++)");
          source.Add("{");
          source.Add("}");
          break;
      }

      // ソースコード作成
      var targetSource = new StringBuilder();
      targetSource.AppendLine("public class Standard");
      targetSource.AppendLine("{");
      targetSource.AppendLine("  public void TestMethod()");
      targetSource.AppendLine("  {");
      source.ForEach(line => targetSource.AppendLine($"    {line }"));
      targetSource.AppendLine("  }");

      addMember.ForEach(line => targetSource.AppendLine($"  {line }"));
      targetSource.AppendLine("}");
      targetSource.AppendLine(addSource.ToString());

      return new FileData(filePath, usingList.ToString(), targetSource.ToString());
    }

    /// <summary>
    /// Setup
    /// </summary>
    public ForTest() : base()
    {
      System.Diagnostics.Debug.WriteLine($"Setup {Environment.CurrentDirectory}");
    }

    /// <summary>
    /// 基本的なテスト
    /// </summary>
    [Fact(DisplayName = "Standard")]
    public void StandardTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.Standard), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "Standard.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemFor;

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 宣言部の確認
        CheckDeclarationsCount(targetInstance, "int", 
          new List<string>() {
            "index=0",
          });

        // 条件の確認
        Assert.Equal("index<10", GetExpressionsToString(targetInstance.Conditions));

        // 計算部の確認
        CheckIncrementorsCount(targetInstance, 
          new List<string>() {
            "index++" 
          });
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// ローカルフィールド参照テスト
    /// </summary>
    [Fact(DisplayName = "LocalField")]
    public void LocalFieldTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.LocalField), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "LocalField.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemFor;

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 宣言部の確認
        CheckDeclarationsCount(targetInstance,string.Empty, 
          new List<string>() {
            "index=0",
          });

        // 条件の確認
        Assert.Equal("index<10", GetExpressionsToString(targetInstance.Conditions));

        // 計算部の確認
        CheckIncrementorsCount(targetInstance,
          new List<string>() {
            "index++"
          });
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// 複数宣言テスト
    /// </summary>
    [Fact(DisplayName = "MultiDeclarations")]
    public void MultiDeclarationsTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.MultiDeclarations), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "MultiDeclarations.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemFor;

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 宣言部の確認
        CheckDeclarationsCount(targetInstance,"int", 
          new List<string>() {
            "a=0",
            "b=0",
          });

        // 条件の確認
        Assert.Equal("a<10", GetExpressionsToString(targetInstance.Conditions));

        // 計算部の確認
        CheckIncrementorsCount(targetInstance,
          new List<string>() {
            "a++",
            "b++",
          });
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// 複数計算テスト
    /// </summary>
    [Fact(DisplayName = "MultiIncrementors")]
    public void MultiIncrementorsTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.MultiIncrementors), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "MultiIncrementors.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemFor;

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 宣言部の確認
        CheckDeclarationsCount(targetInstance, "int",
          new List<string>() {
            "a=0",
          });

        // 条件の確認
        Assert.Equal("a<10", GetExpressionsToString(targetInstance.Conditions));

        // 計算部の確認
        CheckIncrementorsCount(targetInstance,
          new List<string>() {
            "a++",
            "b+=1",
          });
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// メソッド使用のテスト
    /// </summary>
    [Fact(DisplayName = "UseMethods")]
    public void UseMethodsTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.UseMethods), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "UseMethods.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemFor;

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 宣言部の確認
        CheckDeclarationsCount(targetInstance, "int",
          new List<string>() {
            "index=ini()",
          });

        // 条件の確認
        Assert.Equal("condition(index)", GetExpressionsToString(targetInstance.Conditions));

        // 計算部の確認
        CheckIncrementorsCount(targetInstance,
          new List<string>() {
            "index+=incrementor()"
          });
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// プロパティ参照のテスト
    /// </summary>
    [Fact(DisplayName = "InstanceProperty")]
    public void InstancePropertyTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.InstanceProperty), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "InstanceProperty.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemFor;

        // 外部参照の存在確認
        Assert.Single(ev.FileRoot.OtherFiles);
        Assert.Equal("List", ev.FileRoot.OtherFiles.First().Key);

        // 宣言部の確認
        CheckDeclarationsCount(targetInstance, "int",
          new List<string>() {
            "index=0",
          });

        // 条件の確認
        Assert.Equal("index<this.Values.Count", GetExpressionsToString(targetInstance.Conditions));

        // 計算部の確認
        CheckIncrementorsCount(targetInstance,
          new List<string>() {
            "index++"
          });
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// メソッド参照のテスト
    /// </summary>
    [Fact(DisplayName = "InstanceMethod")]
    public void InstanceMethodTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.InstanceMethod), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "InstanceMethod.cs");

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Equal(2, targetInstances.Count);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemFor;

        // 外部参照の存在確認
        Assert.Single(ev.FileRoot.OtherFiles);
        Assert.Equal("List", ev.FileRoot.OtherFiles.First().Key);

        // 宣言部の確認
        CheckDeclarationsCount(targetInstance, "int",
          new List<string>() {
            "index=0",
          });

        // 条件の確認
        Assert.Equal("index<this.GetValues().Count", GetExpressionsToString(targetInstance.Conditions));

        // 計算部の確認
        CheckIncrementorsCount(targetInstance,
          new List<string>() {
            "index++"
          });
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    #region メソッドユーティリティ
    /// <summary>
    /// 対象インスタンスの取得
    /// </summary>
    /// <param name="itemClass">対象のアイテムクラス</param>
    /// <returns>対象インスタンスリスト</returns>
    private List<IItemMethod> GetTargetInstances(IItemClass itemClass)
    {
      return itemClass.Members.Where(member => member is IItemMethod).
              Select(member => member as IItemMethod).ToList();
    }

    /// <summary>
    /// 対象インスタンスの取得
    /// </summary>
    /// <param name="itemClass">対象のアイテムメソッド</param>
    /// <returns>対象インスタンスリスト</returns>
    private List<IItemFor> GetTargetInstances(IItemMethod itemMethod)
    {
      return itemMethod.Members.Where(member => member is IItemFor).
              Select(member => member as IItemFor).ToList();
    }

    /// <summary>
    /// 宣言部の確認
    /// </summary>
    /// <param name="targetInstance">対象のインスタンス</param>
    /// <param name="expectedList">パラメータの期待値<</param>
    private void CheckDeclarationsCount(IItemFor targetInstance, string expectedType, List<string> expectedList)
    {
      // 期待値とローカルフィールドの数が同じか確認
      Assert.Equal(expectedList.Count, targetInstance.Declarations.Count);

      // 宣言部の型の確認
      var actualType = string.Empty;
      if (targetInstance.Types.Any()){
        Assert.Single(targetInstance.Types);
        actualType = targetInstance.Types.Single().Name;
      }
      Assert.Equal(expectedType, actualType);

      // ローカルフィールドの数を取得
      var existsCount = 0;
      for (var index = 0; index < targetInstance.Declarations.Count; index++)
      {
        var expectedTargets = 
              expectedList.Where(item => item == GetExpressionsToString(targetInstance.Declarations[index]));

        if (expectedTargets.Any())
        {
          existsCount++;
        }
      }
      Assert.Equal(existsCount, targetInstance.Declarations.Count);
    }

    /// <summary>
    /// 計算部の確認
    /// </summary>
    /// <param name="targetInstance">対象のインスタンス</param>
    /// <param name="expectedList">パラメータの期待値<</param>
    private void CheckIncrementorsCount(IItemFor targetInstance, List<string> expectedList)
    {
      // 数が同じか確認
      Assert.Equal(expectedList.Count, targetInstance.Incrementors.Count);

      // 対象を文字列に変換
      var actualList = targetInstance.Incrementors.Select(item => GetExpressionsToString(item));

      // 同じリストか確認
      Assert.Equal(expectedList, actualList);
    }
    #endregion

  }
}
