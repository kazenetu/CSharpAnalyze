using CSharpAnalyze.Domain.PublicInterfaces;
using CSharpAnalyze.Domain.PublicInterfaces.AnalyzeItems;
using CSharpAnalyzeTest.Common;
using System;
using System.Text;
using Xunit;

namespace CSharpAnalyzeTest
{
  [Trait("�N���X�̃e�X�g",nameof(ClassTest))]
  public class ClassTest : TestBase
  {
    /// <summary>
    /// �\�[�X�쐬�p�^�[��
    /// </summary>
    private enum CreatePattern
    {
      Standard,
      SubClass,
      InnerClass
    }

    /// <summary>
    /// �t�@�C�����A�\�[�X�R�[�h�擾����
    /// </summary>
    /// <param name="pattern">�����p�^�[��</param>
    /// <returns>�t�@�C���p�X�ƃ\�[�X�R�[�h</returns>
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
      }

      return new FileData(filePath, usingList.ToString(), source.ToString());
    }

    /// <summary>
    /// Setup
    /// </summary>
    public ClassTest():base()
    {
      System.Diagnostics.Debug.WriteLine($"Setup {Environment.CurrentDirectory}");
    }

    [Fact(DisplayName = "Standard")]
    public void StandardTest()
    {
      // �e�X�g�R�[�h��ǉ�
      CreateFileData(CreateSource(CreatePattern.Standard), (ev) =>
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
