using CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems;
using CSharpAnalyzeTest.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CSharpAnalyzeTest
{
  [Trait("Switchのテスト", nameof(SwitchTest))]
  public class SwitchTest : TestBase
  {
    /// <summary>
    /// ソース作成パターン
    /// </summary>
    private enum CreatePattern
    {
      Standard,
      CaseDefault,
      MultiCase,
      TypeCase,
      NestSwitch,
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

          source.Add("var val=1;");
          source.Add("switch(val)");
          source.Add("{");
          source.Add("  case 1:");
          source.Add("  break;");
          source.Add("}");
          break;

        case CreatePattern.CaseDefault:
          filePath = "CaseDefault.cs";

          source.Add("var val=1;");
          source.Add("switch(val)");
          source.Add("{");
          source.Add("  case 1:");
          source.Add("  break;");
          source.Add("  default:");
          source.Add("  break;");
          source.Add("}");
          break;

        case CreatePattern.MultiCase:
          filePath = "MultiCase.cs";

          source.Add("var val=1;");
          source.Add("switch(val)");
          source.Add("{");
          source.Add("  case 1:");
          source.Add("  case 2:");
          source.Add("  break;");
          source.Add("}");
          break;

        case CreatePattern.TypeCase:
          filePath = "TypeCase.cs";

          addMember.Add("private enum EnumTest");
          addMember.Add("{");
          addMember.Add("  Test;");
          addMember.Add("}");

          source.Add("object val = 1;");
          source.Add("switch(val)");
          source.Add("{");
          source.Add("  case int b:");
          source.Add("  break;");
          source.Add("  case string b:");
          source.Add("  break;");
          source.Add("  case EnumTest e:");
          source.Add("  break;");
          source.Add("}");
          break;

        case CreatePattern.NestSwitch:
          filePath = "NestSwitch.cs";

          source.Add("object val = 1;");
          source.Add("switch(val)");
          source.Add("{");
          source.Add("  case int b:");
          source.Add("    switch(b)");
          source.Add("    {");
          source.Add("      case 1:");
          source.Add("      break;");
          source.Add(    "}");
          source.Add("  break;");
          source.Add("}");
          break;

        case CreatePattern.InstanceProperty:
          filePath = "InstanceProperty.cs";

          addMember.Add("private int Val{get;}= 1");

          source.Add("switch(Val)");
          source.Add("{");
          source.Add("  case 1:");
          source.Add("  break;");
          source.Add("}");
          break;

        case CreatePattern.InstanceMethod:
          filePath = "InstanceMethod.cs";

          addMember.Add("private int GetValue()");
          addMember.Add("{");
          addMember.Add("  return 1;");
          addMember.Add("{");

          source.Add("switch(GetValue())");
          source.Add("{");
          source.Add("  case 1:");
          source.Add("  break;");
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
    public SwitchTest() : base()
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

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemSwitch;

        // 分岐構造の確認
        checkSwitch(targetInstance, "val", 1);

        var caseLables = new List<string>()
        {
          "1",
        };
        checkSwitchCase(targetInstance.Cases.First() as IItemSwitchCase, caseLables);
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// defaultありテスト
    /// </summary>
    [Fact(DisplayName = "CaseDefault")]
    public void CaseDefaultTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.CaseDefault), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "CaseDefault.cs");

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemSwitch;

        // 分岐構造の確認
        checkSwitch(targetInstance, "val", 2);

        var caseLablesList = new List<List<string>>()
        {
          new List<string>(){"1" },
          new List<string>(){"default" },
        };
        var caseIndex = 0;
        foreach(IItemSwitchCase itemCase in targetInstance.Cases){
          checkSwitchCase(itemCase, caseLablesList[caseIndex++]);
        }
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// Caseまとまりのテスト
    /// </summary>
    [Fact(DisplayName = "MultiCase")]
    public void MultiCaseTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.MultiCase), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "MultiCase.cs");

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemSwitch;

        // 分岐構造の確認
        checkSwitch(targetInstance, "val", 1);

        var caseLablesList = new List<List<string>>()
        {
          new List<string>(){"1" ,"2"},
        };
        var caseIndex = 0;
        foreach (IItemSwitchCase itemCase in targetInstance.Cases)
        {
          checkSwitchCase(itemCase, caseLablesList[caseIndex++]);
        }
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// 型による分岐のテスト
    /// </summary>
    [Fact(DisplayName = "TypeCase")]
    public void TypeCaseTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.TypeCase), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "TypeCase.cs");

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemSwitch;

        // 分岐構造の確認
        checkSwitch(targetInstance, "val", 3);

        var caseLablesList = new List<List<string>>()
        {
          new List<string>(){"int b"},
          new List<string>(){"string b"},
          new List<string>(){"EnumTest e"},
        };
        var caseIndex = 0;
        foreach (IItemSwitchCase itemCase in targetInstance.Cases)
        {
          checkSwitchCase(itemCase, caseLablesList[caseIndex++]);
        }
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// 分岐のネストのテスト
    /// </summary>
    [Fact(DisplayName = "NestSwitch")]
    public void NestSwitchTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.NestSwitch), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "NestSwitch.cs");

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemSwitch;

        // 分岐構造の確認
        checkSwitch(targetInstance, "val", 1);

        var caseLablesList = new List<List<string>>()
        {
          new List<string>(){"int b"},
        };
        var caseIndex = 0;
        foreach (IItemSwitchCase itemCase in targetInstance.Cases)
        {
          checkSwitchCase(itemCase, caseLablesList[caseIndex++]);
        }

        // caseのメンバーの確認
        var caseBlockMembers = targetInstance.Cases.First().Members;
        Assert.Equal(2, caseBlockMembers.Count);

        // ネスト内の分岐構造の確認
        var innerSwitch = caseBlockMembers.First() as IItemSwitch;
        Assert.NotNull(innerSwitch);
        checkSwitch(innerSwitch, "b", 1);

        Assert.NotEmpty(innerSwitch.Cases);
        checkSwitchCase(innerSwitch.Cases.First() as IItemSwitchCase, 
                        new List<string>() { "1" });

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

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemSwitch;

        // 分岐構造の確認
        checkSwitch(targetInstance, "this.Val", 1);

        var caseLables = new List<string>()
        {
          "1",
        };
        checkSwitchCase(targetInstance.Cases.First() as IItemSwitchCase, caseLables);
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

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Equal(2, targetInstances.Count);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).First() as IItemSwitch;

        // 分岐構造の確認
        checkSwitch(targetInstance, "this.GetValue()", 1);

        var caseLables = new List<string>()
        {
          "1",
        };
        checkSwitchCase(targetInstance.Cases.First() as IItemSwitchCase, caseLables);
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    #region ユーティリティメソッド
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
    private List<IItemSwitch> GetTargetInstances(IItemMethod itemMethod)
    {
      return itemMethod.Members.Where(member => member is IItemSwitch).
              Select(member => member as IItemSwitch).ToList();
    }

    /// <summary>
    /// Switchステートメントの確認
    /// </summary>
    /// <param name="target">対象インスタンス</param>
    /// <param name="condition">条件式</param>
    /// <param name="caseCount">case数</param>
    /// <returns></returns>
    private bool checkSwitch(IItemSwitch target, string condition, int caseCount)
    {
      Assert.NotNull(target);
      Assert.Equal(condition, GetExpressionsToString(target.Conditions));
      Assert.Equal(caseCount, target.Cases.Count);

      return true;
    }

    /// <summary>
    /// SwitchCaseテートメントの確認
    /// </summary>
    /// <param name="target">対象インスタンス</param>
    /// <param name="labels">条件式</param>
    private bool checkSwitchCase(IItemSwitchCase target, List<string> labels)
    {
      Assert.NotNull(target);
      var actualLables = target.Labels.Select(item => GetExpressionsToString(item));
      Assert.Equal(labels, actualLables);
      Assert.Equal(labels.Count, actualLables.Count());

      return true;
    }


    #endregion

  }
}
