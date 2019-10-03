using CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems;
using CSharpAnalyzeTest.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CSharpAnalyzeTest
{
  [Trait("If Elseのテスト", nameof(IfElseTest))]
  public class IfElseTest : TestBase
  {
    /// <summary>
    /// ソース作成パターン
    /// </summary>
    private enum CreatePattern
    {
      Standard,
      ExistsElse,
      ExistsManyElse,
      NestIfElse,
      RefLocalField,
      ConditionsType,
      ConditionsIsType,
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

          source.Add("if(true)");
          source.Add("{");
          source.Add("}");
          break;

        case CreatePattern.ExistsElse:
          filePath = "ExistsElse.cs";

          source.Add("if(true)");
          source.Add("{");
          source.Add("}");
          source.Add("else");
          source.Add("{");
          source.Add("}");
          break;

        case CreatePattern.ExistsManyElse:
          filePath = "ExistsManyElse.cs";

          source.Add("if(true)");
          source.Add("{");
          source.Add("}");
          source.Add("else if(1==1)");
          source.Add("{");
          source.Add("}");
          source.Add("else");
          source.Add("{");
          source.Add("}");
          break;

        case CreatePattern.NestIfElse:
          filePath = "NestIfElse.cs";

          source.Add("if(true)");
          source.Add("{");
          source.Add("  if(1==1)");
          source.Add("  {");
          source.Add("  }");
          source.Add("  else");
          source.Add("  {");
          source.Add("  }");
          source.Add("}");
          break;

        case CreatePattern.RefLocalField:
          filePath = "RefLocalField.cs";

          source.Add("var val=true;");
          source.Add("if(val)");
          source.Add("{");
          source.Add("}");
          break;

        case CreatePattern.ConditionsType:
          filePath = "ConditionsType.cs";

          source.Add("var val=1;");
          source.Add("if(val is string b)");
          source.Add("{");
          source.Add("}");
          break;

        case CreatePattern.ConditionsIsType:
          filePath = "ConditionsIsType.cs";

          source.Add("var val=1;");
          source.Add("if(val is string)");
          source.Add("{");
          source.Add("}");
          break;

        case CreatePattern.InstanceProperty:
          filePath = "InstanceProperty.cs";

          addMember.Add("private int Val{get;}= 1");

          source.Add("if(Val == 1)");
          source.Add("{");
          source.Add("}");
          break;

        case CreatePattern.InstanceMethod:
          filePath = "InstanceMethod.cs";

          addMember.Add("private bool GetValue()");
          addMember.Add("{");
          addMember.Add("  return true;");
          addMember.Add("{");

          source.Add("if(GetValue())");
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
    public IfElseTest() : base()
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
        var targetInstance = GetTargetInstances(targetParentInstance).FirstOrDefault() as IItemIf;

        // 対象インスタンスの存在確認
        Assert.NotNull(targetInstance);

        // 分岐構造の確認
        checkIf(targetInstance, "true", 0);
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// Else付き：条件なし
    /// </summary>
    [Fact(DisplayName = "ExistsElse")]
    public void ExistsElseTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.ExistsElse), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "ExistsElse.cs");

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).FirstOrDefault() as IItemIf;

        // 対象インスタンスの存在確認
        Assert.NotNull(targetInstance);

        // 分岐構造の確認
        checkIf(targetInstance, "true", 1);

        Assert.Single(targetInstance.FalseBlocks);
        checkElse(targetInstance.FalseBlocks.First() as IItemElseClause, "");

      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// Else付き：複数条件
    /// </summary>
    [Fact(DisplayName = "ExistsManyElse")]
    public void ExistsManyElseTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.ExistsManyElse), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "ExistsManyElse.cs");

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).FirstOrDefault() as IItemIf;

        // 対象インスタンスの存在確認
        Assert.NotNull(targetInstance);

        // 分岐構造の確認
        checkIf(targetInstance, "true", 2);

        var elseConditions = new List<string>()
        {
          "1==1",
          "",
        };
        var elseConditionIndex = 0;
        foreach (var elseItem in targetInstance.FalseBlocks){
          checkElse(elseItem as IItemElseClause, elseConditions[elseConditionIndex]);
          elseConditionIndex++;
        }
        Assert.True(targetInstance.FalseBlocks.Count == elseConditionIndex);
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// IfElseのネスト
    /// </summary>
    [Fact(DisplayName = "NestIfElse")]
    public void NestIfElseTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.NestIfElse), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "NestIfElse.cs");

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).FirstOrDefault() as IItemIf;

        // 対象インスタンスの存在確認
        Assert.NotNull(targetInstance);

        // 分岐構造の確認
        checkIf(targetInstance, "true", 0);

        // 内部の分岐構造の確認
        Assert.NotEmpty(targetInstance.TrueBlock);
        var innerItemIf = targetInstance.TrueBlock.First() as IItemIf;
        checkIf(innerItemIf, "1==1", 1);

        Assert.NotEmpty(innerItemIf.FalseBlocks);
        checkElse(innerItemIf.FalseBlocks.First() as IItemElseClause, "");
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// ローカルフィールド参照
    /// </summary>
    [Fact(DisplayName = "RefLocalField")]
    public void RefLocalFieldTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.RefLocalField), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "RefLocalField.cs");

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).FirstOrDefault() as IItemIf;

        // 対象インスタンスの存在確認
        Assert.NotNull(targetInstance);

        // 分岐構造の確認
        checkIf(targetInstance, "val", 0);
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// 型による分岐
    /// </summary>
    [Fact(DisplayName = "ConditionsType")]
    public void ConditionsTypeTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.ConditionsType), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "ConditionsType.cs");

        // 外部参照の存在確認
        Assert.Empty(ev.FileRoot.OtherFiles);

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).FirstOrDefault() as IItemIf;

        // 対象インスタンスの存在確認
        Assert.NotNull(targetInstance);

        // 分岐構造の確認
        checkIf(targetInstance, "val is string b", 0);
      });

      // 解析実行
      CSAnalyze.Analyze(string.Empty, Files);
    }

    /// <summary>
    /// 型による分岐
    /// </summary>
    [Fact(DisplayName = "ConditionsIsTypeTest")]
    public void ConditionsIsTypeTest()
    {
      // テストコードを追加
      CreateFileData(CreateSource(CreatePattern.ConditionsIsType), (ev) =>
      {
        // IItemClassインスタンスを取得
        var itemClass = GetClassInstance(ev, "ConditionsIsType.cs");

        // 外部参照の存在確認
        Assert.Single(ev.FileRoot.OtherFiles);

        // 対象インスタンスのリストを取得
        var targetInstances = GetTargetInstances(itemClass);

        // 対象の親インスタンスを取得
        Assert.Single(targetInstances);
        var targetParentInstance = targetInstances.First() as IItemMethod;

        // 対象インスタンスを取得
        var targetInstance = GetTargetInstances(targetParentInstance).FirstOrDefault() as IItemIf;

        // 対象インスタンスの存在確認
        Assert.NotNull(targetInstance);

        // 分岐構造の確認
        checkIf(targetInstance, "val is string", 0);
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
        var targetInstance = GetTargetInstances(targetParentInstance).FirstOrDefault() as IItemIf;

        // 対象インスタンスの存在確認
        Assert.NotNull(targetInstance);

        // 分岐構造の確認
        checkIf(targetInstance, "this.Val==1", 0);
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
        var targetInstance = GetTargetInstances(targetParentInstance).FirstOrDefault() as IItemIf;

        // 対象インスタンスの存在確認
        Assert.NotNull(targetInstance);

        // 分岐構造の確認
        checkIf(targetInstance, "this.GetValue()", 0);
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
    private List<IItemIf> GetTargetInstances(IItemMethod itemMethod)
    {
      return itemMethod.Members.Where(member => member is IItemIf).
        Select(member => member as IItemIf).ToList();
    }

    /// <summary>
    /// IFステートメントの確認
    /// </summary>
    /// <param name="target">対象インスタンス</param>
    /// <param name="condition">条件式</param>
    /// <param name="existElseBlock">else数</param>
    /// <returns></returns>
    private bool checkIf(IItemIf target,string condition,int elseBlockCount)
    {
      Assert.NotNull(target);
      Assert.Equal(condition, GetExpressionsToString(target.Conditions));
      Assert.Equal(elseBlockCount, target.FalseBlocks.Count);

      return true;
    }

    /// <summary>
    /// ELSEステートメントの確認
    /// </summary>
    /// <param name="target">対象インスタンス</param>
    /// <param name="condition">条件式</param>
    private bool checkElse(IItemElseClause target, string condition)
    {
      Assert.NotNull(target);
      Assert.Equal(condition, GetExpressionsToString(target.Conditions));

      return true;
    }
    #endregion

  }
}
