using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;

namespace GCScript.ExtensionMethods;

public enum ETextCase { None, ToLower, ToUpper, ToTitleCase }
public enum ETextType { None, OnlyLetters, OnlyNumbers, OnlyLettersNumbers, OnlyLettersNumbersSpaces }
public enum ETextTrim { None, Trim, TrimStart, TrimEnd }
public enum ETextRemoveSpaces { None, Duplicate, All }

public static class GCScriptExtensionMethods
{
    /// <summary>
    /// Process text with multiple options.
    /// </summary>
    /// <param name="text">Text to be processed.</param>
    /// <param name="removeAccents">Remove accents from text.</param>
    /// <param name="textTrim">Trim text.</param>
    /// <param name="textCase">Change text case.</param>
    /// <param name="textType">Get only letters, numbers, letters and numbers or letters, numbers and spaces.</param>
    /// <param name="removeSpaces">Remove duplicate or all spaces.</param>
    /// <returns>Processed text.</returns>
    public static string ProcessText(this string? text,
                                     bool removeAccents = true,
                                     ETextTrim textTrim = ETextTrim.Trim,
                                     ETextCase textCase = ETextCase.ToLower,
                                     ETextType textType = ETextType.None,
                                     ETextRemoveSpaces removeSpaces = ETextRemoveSpaces.Duplicate)
    {
        if (string.IsNullOrWhiteSpace(text)) { return ""; }

        if (removeAccents) { text = text.RemoveAccents(); }

        switch (textTrim)
        {
            case ETextTrim.Trim: { text = text.Trim(); break; }
            case ETextTrim.TrimStart: { text = text.TrimStart(); break; }
            case ETextTrim.TrimEnd: { text = text.TrimEnd(); break; }
        }

        switch (textCase)
        {
            case ETextCase.ToLower: { text = text.ToLower(); break; }
            case ETextCase.ToUpper: { text = text.ToUpper(); break; }
            case ETextCase.ToTitleCase: { text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower()); break; }
        }

        switch (removeSpaces)
        {
            case ETextRemoveSpaces.Duplicate: { text = text.RemoveDuplicateSpaces(); break; }
            case ETextRemoveSpaces.All: { text = text.RemoveSpaces(); break; }
        }

        switch (textType)
        {
            case ETextType.OnlyLetters: { text = text.OnlyLetters(); break; }
            case ETextType.OnlyNumbers: { text = text.OnlyNumbers(); break; }
            case ETextType.OnlyLettersNumbers: { text = text.OnlyLettersNumbers(); break; }
            case ETextType.OnlyLettersNumbersSpaces: { text = text.OnlyLettersNumbersSpaces(); break; }
        }

        return text;
    }

    private static string RemoveAccents(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) { return ""; }
        StringBuilder sbReturn = new StringBuilder();
        foreach (char letter in text.Normalize(NormalizationForm.FormD))
        {
            if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)
                sbReturn.Append(letter);
        }
        return sbReturn.ToString();
    }

    private static string RemoveSpaces(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) { return ""; }
        text = Regex.Replace(text, @"\s", "");
        return text;
    }

    private static string RemoveDuplicateSpaces(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) { return ""; }
        text = Regex.Replace(text, @"\s+", " ");
        return text;
    }

    private static string OnlyLetters(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) { return ""; }
        text = Regex.Replace(text, @"[^a-zA-Z]", "");
        return text;
    }

    private static string OnlyNumbers(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) { return ""; }
        text = Regex.Replace(text, @"[^0-9]", "");
        return text;
    }

    private static string OnlyLettersNumbers(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) { return ""; }
        text = Regex.Replace(text, @"[^a-zA-Z0-9]", "");
        return text;
    }

    private static string OnlyLettersNumbersSpaces(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) { return ""; }
        text = Regex.Replace(text, @"[^a-zA-Z0-9\s]", "");
        return text;
    }
}
