using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;
using SimMetrics.Net.Metric;
using GCScript.ExtensionMethods.Enums;
using GCScript.ExtensionMethods.Models;

namespace GCScript.ExtensionMethods;

public static class GCScriptExtensionMethods
{
    /// <summary>
    /// Processes a given text based on specified options.
    /// </summary>
    /// <param name="text">The input text to be processed.</param>
    /// <param name="removeAccents">A boolean indicating whether to remove accents from the text.</param>
    /// <param name="textTrim">An enum specifying the trimming options for the text.</param>
    /// <param name="textCase">An enum specifying the case transformation options for the text.</param>
    /// <param name="textType">An enum specifying the type of characters to keep in the text.</param>
    /// <param name="removeSpaces">An enum specifying the space removal options for the text.</param>
    /// <returns>The processed text based on the specified options.</returns>
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

    public static double GetStringSimilarityPercentage(this string? text, string? textToCompare, StringSimilarityOptions? options = null)
    {
        options ??= new StringSimilarityOptions();
        if (options.ProcessText) { text = text.ProcessText(); textToCompare = textToCompare.ProcessText(); }
        if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(textToCompare)) { return 0; }
        List<(string Method, double Similarity)> similarities = new();
        if (options.Levenstein) { similarities.Add((Method: "Levenstein", Similarity: new Levenstein().GetSimilarity(text, textToCompare))); }
        if (options.JaroWinkler) { similarities.Add((Method: "JaroWinkler", Similarity: new JaroWinkler().GetSimilarity(text, textToCompare))); }
        if (options.Jaccard) { similarities.Add((Method: "Jaccard", Similarity: new JaccardSimilarity().GetSimilarity(text, textToCompare))); }
        return Math.Round(similarities.Average(x => x.Similarity) * 100, 2);
    }

    public static List<(string Method, double Percentage)> GetStringListSimilarityPercentages(this string? text, List<string> listToCompare, StringSimilarityOptions? options = null)
    {
        options ??= new StringSimilarityOptions();
        if (options.ProcessText) { text = text.ProcessText(); listToCompare = listToCompare.Select(x => x.ProcessText()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList(); }
        else { listToCompare = listToCompare.Where(x => !string.IsNullOrWhiteSpace(x)).ToList(); }

        if (string.IsNullOrWhiteSpace(text) || listToCompare.Count == 0) { return new(); }
        List<(string Method, double Percentage)> similarities = new();
        foreach (string item in listToCompare)
        {
            List<(string Method, double Similarity)> similaritiesItem = new();
            if (options.Levenstein) { similaritiesItem.Add((Method: "Levenstein", Similarity: new Levenstein().GetSimilarity(text, item))); }
            if (options.JaroWinkler) { similaritiesItem.Add((Method: "JaroWinkler", Similarity: new JaroWinkler().GetSimilarity(text, item))); }
            if (options.Jaccard) { similaritiesItem.Add((Method: "Jaccard", Similarity: new JaccardSimilarity().GetSimilarity(text, item))); }
            similarities.Add((Method: item, Percentage: Math.Round(similaritiesItem.Average(x => x.Similarity) * 100, 2)));
        }
        return [.. similarities.OrderByDescending(x => x.Percentage).ThenBy(x => x.Method).ToArray()];
    }
}
