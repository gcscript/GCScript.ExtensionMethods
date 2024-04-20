using GCScript.ExtensionMethods;
using GCScript.ExtensionMethods.Enums;

namespace GCScript.ExtensionMethodsTest;

public class GCScriptExtensionMethodsTest
{
    [Fact(DisplayName = "ProcessText - Default Text Processing")]
    public void ShouldProcessTextWithDefaultOptions()
    {
        // Arrange
        var input = "   Text   áéíóú 123   ";
        var expected = "text aeiou 123";

        // Act
        var result = input.ProcessText();

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact(DisplayName = "ProcessText - Accent Removal")]
    public void ShouldRemoveAccentsFromText()
    {
        // Arrange
        var input = "áéíóú";
        var expected = "aeiou";

        // Act
        var result = input.ProcessText(removeAccents: true);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact(DisplayName = "ProcessText - Trimming")]
    public void ShouldTestTrimmingVariations()
    {
        // Arrange
        var input = "   text   ";
        var trimExpected = "text";
        var trimStartExpected = "text   ";
        var trimEndExpected = "   text";

        // Act
        var trimResult = input.ProcessText(removeAccents: false, textTrim: ETextTrim.Trim, textCase: ETextCase.None, textType: ETextType.None, removeSpaces: ETextRemoveSpaces.None);
        var trimStartResult = input.ProcessText(removeAccents: false, textTrim: ETextTrim.TrimStart, textCase: ETextCase.None, textType: ETextType.None, removeSpaces: ETextRemoveSpaces.None);
        var trimEndResult = input.ProcessText(removeAccents: false, textTrim: ETextTrim.TrimEnd, textCase: ETextCase.None, textType: ETextType.None, removeSpaces: ETextRemoveSpaces.None);

        // Assert
        Assert.Equal(trimExpected, trimResult);
        Assert.Equal(trimStartExpected, trimStartResult);
        Assert.Equal(trimEndExpected, trimEndResult);
    }

    [Fact(DisplayName = "ProcessText - Text Case Transformation")]
    public void ShouldTestTextCaseTransformationVariations()
    {
        // Arrange
        var input = "TeXt";
        var toLowerExpected = "text";
        var toUpperExpected = "TEXT";
        var toTitleCaseExpected = "Text";

        // Act
        var toLowerResult = input.ProcessText(removeAccents: false, textTrim: ETextTrim.None, textCase: ETextCase.ToLower, textType: ETextType.None, removeSpaces: ETextRemoveSpaces.None);
        var toUpperResult = input.ProcessText(removeAccents: false, textTrim: ETextTrim.None, textCase: ETextCase.ToUpper, textType: ETextType.None, removeSpaces: ETextRemoveSpaces.None);
        var toTitleCaseResult = input.ProcessText(removeAccents: false, textTrim: ETextTrim.None, textCase: ETextCase.ToTitleCase, textType: ETextType.None, removeSpaces: ETextRemoveSpaces.None);

        // Assert
        Assert.Equal(toLowerExpected, toLowerResult);
        Assert.Equal(toUpperExpected, toUpperResult);
        Assert.Equal(toTitleCaseExpected, toTitleCaseResult);
    }

    [Fact(DisplayName = "ProcessText - Text Type Variations")]
    public void ShouldTestTextTypeVariations()
    {
        // Arrange
        var input = "text 123 !@#";
        var onlyLettersExpected = "text";
        var onlyNumbersExpected = "123";
        var onlyLettersNumbersExpected = "text123";
        var onlyLettersNumbersSpacesExpected = "text 123 ";

        // Act
        var onlyLettersResult = input.ProcessText(removeAccents: false, textTrim: ETextTrim.None, textCase: ETextCase.None, textType: ETextType.OnlyLetters, removeSpaces: ETextRemoveSpaces.None);
        var onlyNumbersResult = input.ProcessText(removeAccents: false, textTrim: ETextTrim.None, textCase: ETextCase.None, textType: ETextType.OnlyNumbers, removeSpaces: ETextRemoveSpaces.None);
        var onlyLettersNumbersResult = input.ProcessText(removeAccents: false, textTrim: ETextTrim.None, textCase: ETextCase.None, textType: ETextType.OnlyLettersNumbers, removeSpaces: ETextRemoveSpaces.None);
        var onlyLettersNumbersSpacesResult = input.ProcessText(removeAccents: false, textTrim: ETextTrim.None, textCase: ETextCase.None, textType: ETextType.OnlyLettersNumbersSpaces, removeSpaces: ETextRemoveSpaces.None);

        // Assert
        Assert.Equal(onlyLettersExpected, onlyLettersResult);
        Assert.Equal(onlyNumbersExpected, onlyNumbersResult);
        Assert.Equal(onlyLettersNumbersExpected, onlyLettersNumbersResult);
        Assert.Equal(onlyLettersNumbersSpacesExpected, onlyLettersNumbersSpacesResult);
    }

    [Fact(DisplayName = "ProcessText - Space Removal")]
    public void ShouldTestSpaceRemovalVariations()
    {
        // Arrange
        var input = "text  with   spaces";
        var removeDuplicateSpacesExpected = "text with spaces";
        var removeAllSpacesExpected = "textwithspaces";

        // Act
        var removeDuplicateSpacesResult = input.ProcessText(removeAccents: false, textTrim: ETextTrim.None, textCase: ETextCase.None, textType: ETextType.None, removeSpaces: ETextRemoveSpaces.Duplicate);
        var removeAllSpacesResult = input.ProcessText(removeAccents: false, textTrim: ETextTrim.None, textCase: ETextCase.None, textType: ETextType.None, removeSpaces: ETextRemoveSpaces.All);

        // Assert
        Assert.Equal(removeDuplicateSpacesExpected, removeDuplicateSpacesResult);
        Assert.Equal(removeAllSpacesExpected, removeAllSpacesResult);
    }
}
