using AutoBogus;
using Recipes.Client.Core.ViewModels;
using Recipes.Mobile.TemplateSelectors;

namespace Recipes.Mobile.UnitTests;

public class InstructionsDataTemplateSelectorTests
{
    [Fact]
    public void SelectTemplate_NoteVM_Shoud_Return_NoteTemplate()
    {
        //Arrange
        var template = new DataTemplate();
        var sut = new InstructionsDataTemplateSelector();
        sut.NoteTemplate = template;
        sut.InstructionTemplate = new DataTemplate();

        //Act
        var result = sut.SelectTemplate(AutoFaker.Generate<NoteViewModel>(), null);

        //Assert
        Assert.Equal(template, result);
    }
}
