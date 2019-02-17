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
      // �e�X�g�R�[�h����
      var source = new StringBuilder();
      source.AppendLine("public class ClassTest");
      source.AppendLine("{");
      source.AppendLine("}");

      // �e�X�g�R�[�h��ǉ�
      CreateFileData("Test.cs", "", source.ToString(), (ev) =>
         {
           // �t�@�C�����̊m�F
           Assert.True(ev.FilePath == "Test.cs");

           // ��͌��ʂ̑��݊m�F
           Assert.NotNull(ev.FileRoot);

           // �O���Q�Ƃ̑��݊m�F
           Assert.True(ev.FileRoot.OtherFiles.Count == 0);

           // ��͌��ʂ̌����m�F
           Assert.True(ev.FileRoot.Members.Count == 1);

           // IItemClass�C���X�^���X�̊m�F
           Assert.True(ev.FileRoot.Members[0] is IItemClass);

           // IItemClass�C���X�^���X���擾
           var itemClass = ev.FileRoot.Members[0] as IItemClass;

           // �X�[�p�[�N���X�̐ݒ�m�F
           Assert.True(itemClass.SuperClass.Count == 0);

           // �e�̑��݊m�F
           Assert.Null(itemClass.Parent);

           // �N���X���̊m�F
           Assert.True(itemClass.Name == "ClassTest");

           // �X�R�[�v�C���q�̌����m�F
           Assert.True(itemClass.Modifiers.Count == 1);

           // �X�R�[�v�C���q�̓��e�m�F
           Assert.Contains("public", itemClass.Modifiers);

           // ItemType�̊m�F
           Assert.True(itemClass.ItemType == ItemTypes.Class);

           // �N���X���̗v�f�̑��݊m�F
           Assert.True(itemClass.Members.Count == 0);
         });

      // ��͎��s
      CSAnalyze.Analyze(string.Empty, Files);
    }
  }

}
