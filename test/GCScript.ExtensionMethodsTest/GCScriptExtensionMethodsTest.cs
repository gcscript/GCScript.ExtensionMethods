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

    [Fact(DisplayName = "IsNullOrWhiteSpace - Check If String Is Null or White Space")]
    public void ShouldCheckIfStringIsNullOrWhiteSpace()
    {
        // Arrange
        var input1 = "";
        var input2 = " ";
        var input3 = "abc123";
        var expected1 = true;
        var expected2 = true;
        var expected3 = false;

        // Act
        var result1 = input1.IsNullOrWhiteSpace();
        var result2 = input2.IsNullOrWhiteSpace();
        var result3 = input3.IsNullOrWhiteSpace();

        // Assert
        Assert.Equal(expected1, result1);
        Assert.Equal(expected2, result2);
        Assert.Equal(expected3, result3);
    }

    [Fact(DisplayName = "Left - Extract Left Part of Text")]
    public void ShouldReturnLeftPartOfString()
    {
        // Arrange
        var input1 = "Lorem ipsum dolor sit amet";
        var input2 = "";
        var input3 = "abc123";
        var expected1 = "Lor";
        var expected2 = "";
        var expected3 = "";

        // Act
        var result1 = input1.Left(3);
        var result2 = input2.Left(3);
        var result3 = input3.Left(10);

        // Assert
        Assert.Equal(expected1, result1);
        Assert.Equal(expected2, result2);
        Assert.Equal(expected3, result3);
    }

    [Fact(DisplayName = "Right - Extract Right Part of Text")]
    public void ShouldReturnRightPartOfString()
    {
        // Arrange
        var input1 = "Lorem ipsum dolor sit amet";
        var input2 = "";
        var input3 = "abc123";
        var expected1 = "met";
        var expected2 = "";
        var expected3 = "";

        // Act
        var result1 = input1.Right(3);
        var result2 = input2.Right(3);
        var result3 = input3.Right(10);

        // Assert
        Assert.Equal(expected1, result1);
        Assert.Equal(expected2, result2);
        Assert.Equal(expected3, result3);
    }
}
