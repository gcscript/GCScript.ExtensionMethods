using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;
using SimMetrics.Net.Metric;
using GCScript.ExtensionMethods.Enums;
using GCScript.ExtensionMethods.Models;
using System.Diagnostics.CodeAnalysis;
using GCScript.Shared.Enums;

namespace GCScript.ExtensionMethods;

public static class GCScriptStringExtensions
{
    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? text) => string.IsNullOrWhiteSpace(text);

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
        if (text.IsNullOrWhiteSpace()) { return ""; }

        if (removeAccents) { text = text.RemoveAccents(); }

        switch (textTrim)
        {
            case ETextTrim.Trim: { text = text.Trim(); break; }
            case ETextTrim.TrimStart: { text = text.TrimStart(); break; }
            case ETextTrim.TrimEnd: { text = text.TrimEnd(); break; }
        }

        switch (textCase)
        {
            case ETextCase.ToLower: { text = text.ToLowerInvariant(); break; }
            case ETextCase.ToUpper: { text = text.ToUpperInvariant(); break; }
            case ETextCase.ToTitleCase: { text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLowerInvariant()); break; }
        }

        switch (removeSpaces)
        {
            case ETextRemoveSpaces.Duplicate: { text = text.RemoveDuplicateSpaces(); break; }
            case ETextRemoveSpaces.All: { text = text.RemoveAllSpaces(); break; }
        }

        switch (textType)
        {
            case ETextType.OnlyLetters: { text = text.OnlyLetters(); break; }
            case ETextType.OnlyNumbers: { text = text.OnlyNumbers(); break; }
            case ETextType.OnlyLettersAndNumbers: { text = text.OnlyLettersAndNumbers(); break; }
            case ETextType.OnlyLettersAndNumbersAndSpaces: { text = text.OnlyLettersAndNumbersAndSpaces(); break; }
        }

        return text;
    }

    /// <summary>
    /// Removes accents from the input text.
    /// </summary>
    /// <param name="text">The input text.</param>
    /// <returns>The input text with accents removed.</returns>
    private static string RemoveAccents(this string? text)
    {
        if (text.IsNullOrWhiteSpace()) { return ""; }
        StringBuilder sbReturn = new StringBuilder();
        foreach (char letter in text.Normalize(NormalizationForm.FormD))
        {
            if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)
                sbReturn.Append(letter);
        }
        return sbReturn.ToString();
    }

    /// <summary>
    /// Removes all spaces from the input text.
    /// </summary>
    /// <param name="text">The input text.</param>
    /// <returns>The input text with all spaces removed.</returns>
    private static string RemoveAllSpaces(this string? text)
    {
        if (text.IsNullOrWhiteSpace()) { return ""; }
        text = Regex.Replace(text, @"\s", "");
        return text;
    }

    /// <summary>
    /// Removes duplicate spaces from the input text.
    /// </summary>
    /// <param name="text">The input text.</param>
    /// <returns>The input text with duplicate spaces removed.</returns>
    private static string RemoveDuplicateSpaces(this string? text)
    {
        if (text.IsNullOrWhiteSpace()) { return ""; }
        text = Regex.Replace(text, @"\s+", " ");
        return text;
    }

    /// <summary>
    /// Removes non-alphabetic characters from the input text.
    /// </summary>
    /// <param name="text">The input text.</param>
    /// <returns>The input text with only letters.</returns>
    private static string OnlyLetters(this string? text)
    {
        if (text.IsNullOrWhiteSpace()) { return ""; }
        text = Regex.Replace(text, @"[^a-zA-Z]", "");
        return text;
    }

    /// <summary>
    /// Removes non-numeric characters from the input text.
    /// </summary>
    /// <param name="text">The input text.</param>
    /// <returns>The input text with only numeric characters.</returns>
    private static string OnlyNumbers(this string? text)
    {
        if (text.IsNullOrWhiteSpace()) { return ""; }
        text = Regex.Replace(text, @"[^0-9]", "");
        return text;
    }

    /// <summary>
    /// Removes non-alphanumeric characters from the input text.
    /// </summary>
    /// <param name="text">The input text.</param>
    /// <returns>The input text with only letters and numbers.</returns>
    private static string OnlyLettersAndNumbers(this string? text)
    {
        if (text.IsNullOrWhiteSpace()) { return ""; }
        text = Regex.Replace(text, @"[^a-zA-Z0-9]", "");
        return text;
    }

    /// <summary>
    /// Removes non-alphanumeric characters and spaces from the input text.
    /// </summary>
    /// <param name="text">The input text.</param>
    /// <returns>The input text with only letters, numbers, and spaces.</returns>
    private static string OnlyLettersAndNumbersAndSpaces(this string? text)
    {
        if (text.IsNullOrWhiteSpace()) { return ""; }
        text = Regex.Replace(text, @"[^a-zA-Z0-9\s]", "");
        return text;
    }

    /// <summary>
    /// Retrieves the specified number of characters from the left end of the input text.
    /// </summary>
    /// <param name="text">The input text.</param>
    /// <param name="length">The number of characters to retrieve.</param>
    /// <returns>The substring containing the specified number of characters from the left end of the input text.</returns>
    public static string Left(this string? text, int length)
    {
        if (text.IsNullOrWhiteSpace() || length < 0) { return ""; }
        if (length > text.Length) { return text; }
        return text[..length];
    }

    /// <summary>
    /// Retrieves the specified number of characters from the right end of the input text.
    /// </summary>
    /// <param name="text">The input text.</param>
    /// <param name="length">The number of characters to retrieve.</param>
    /// <returns>The substring containing the specified number of characters from the right end of the input text.</returns>
    public static string Right(this string? text, int length)
    {
        if (text.IsNullOrWhiteSpace() || length < 0) { return ""; }
        if (length > text.Length) { return text; }
        return text[^length..];
    }

    /// <summary>
    /// Converts the input text to a slug format.
    /// </summary>
    /// <param name="text">The input text to be converted.</param>
    /// <returns>The input text converted to a slug format.</returns>
    public static string ToSlug(this string? text)
    {
        text = text.ProcessText(removeAccents: true, textTrim: ETextTrim.Trim, textCase: ETextCase.ToLower, textType: ETextType.OnlyLettersAndNumbersAndSpaces, removeSpaces: ETextRemoveSpaces.Duplicate);
        if (text.IsNullOrWhiteSpace()) { return ""; }
        text = text.Replace(" ", "-");
        return text;
    }

    /// <summary>
    /// Converts the input text to PascalCase format.
    /// </summary>
    /// <param name="text">The input text to be converted.</param>
    /// <returns>The input text converted to PascalCase format.</returns>
    public static string ToPascalCase(this string? text)
    {
        text = text.ProcessText(removeAccents: true, textTrim: ETextTrim.Trim, textCase: ETextCase.ToTitleCase, textType: ETextType.OnlyLettersAndNumbersAndSpaces, removeSpaces: ETextRemoveSpaces.Duplicate);
        if (text.IsNullOrWhiteSpace()) { return ""; }
        text = text.Replace(" ", "");
        return text;
    }

    /// <summary>
    /// Converts the specified text to camel case format.
    /// </summary>
    /// <param name="text">The text to convert to camel case format.</param>
    /// <returns>The specified text converted to camel case format.</returns>
    public static string ToCamelCase(this string? text)
    {
        text = text.ProcessText(removeAccents: true, textTrim: ETextTrim.Trim, textCase: ETextCase.ToTitleCase, textType: ETextType.OnlyLettersAndNumbersAndSpaces, removeSpaces: ETextRemoveSpaces.Duplicate);
        if (text.IsNullOrWhiteSpace()) { return ""; }
        text = text.Replace(" ", "");
        if (text.Length > 1) { text = char.ToLowerInvariant(text[0]) + text[1..]; }
        return text;
    }

    /// <summary>
    /// Counts the number of occurrences of the specified word in the given text.
    /// </summary>
    /// <param name="text">The text to search for occurrences.</param>
    /// <param name="word">The word to count occurrences of.</param>
    /// <returns>The number of occurrences of the specified word in the given text.</returns>
    public static int CountOccurrences(this string text, string word)
    {
        int count = 0;
        int index = 0;
        while ((index = text.IndexOf(word, index)) != -1)
        {
            index += word.Length;
            count++;
        }
        return count;
    }

    /// <summary>
    /// Checks if the specified text contains the specified words with the specified number of occurrences based on the specified comparison type.
    /// </summary>
    /// <param name="text">The text to search for occurrences.</param>
    /// <param name="words">The array of words to search for.</param>
    /// <param name="occurrences">The expected number of occurrences for each word.</param>
    /// <param name="comparisonType">The type of comparison to use for checking occurrences.</param>
    /// <returns>True if the specified text contains the specified words with the specified number of occurrences based on the specified comparison type; otherwise, false.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an invalid comparison type is provided.</exception>
    public static bool CheckOccurrences(this string text, string[] words, int occurrences, EOccurrenceComparisonOperator comparisonType = EOccurrenceComparisonOperator.Equals)
    {
        foreach (string word in words)
        {
            int count = 0;
            int index = 0;
            while ((index = text.IndexOf(word, index)) != -1)
            {
                index += word.Length;
                count++;
            }

            switch (comparisonType)
            {
                case EOccurrenceComparisonOperator.Equals: if (count != occurrences) { return false; } break;
                case EOccurrenceComparisonOperator.NotEquals: if (count == occurrences) { return false; } break;
                case EOccurrenceComparisonOperator.GreaterThan: if (count <= occurrences) { return false; } break;
                case EOccurrenceComparisonOperator.GreaterThanOrEqual: if (count < occurrences) { return false; } break;
                case EOccurrenceComparisonOperator.LessThan: if (count >= occurrences) { return false; } break;
                case EOccurrenceComparisonOperator.LessThanOrEqual: if (count > occurrences) { return false; } break;
                default: throw new ArgumentOutOfRangeException();
            }
        }
        return true;
    }

    /// <summary>
    /// Calculates the similarity percentage between two input texts.
    /// </summary>
    /// <param name="text">The first input text.</param>
    /// <param name="textToCompare">The second input text to compare with the first text.</param>
    /// <param name="options">Options for string similarity calculation. If null, default options are used.</param>
    /// <returns>The similarity percentage between the two input texts.</returns>
    public static double GetStringSimilarityPercentage(this string? text, string? textToCompare, GCScriptStringSimilarityOptions? options = null)
    {
        options ??= new GCScriptStringSimilarityOptions();
        if (options.ProcessText) { text = text.ProcessText(); textToCompare = textToCompare.ProcessText(); }
        if (text.IsNullOrWhiteSpace() || textToCompare.IsNullOrWhiteSpace()) { return 0; }
        List<(string Method, double Similarity)> similarities = new();
        if (options.Levenstein) { similarities.Add((Method: "Levenstein", Similarity: new Levenstein().GetSimilarity(text, textToCompare))); }
        if (options.JaroWinkler) { similarities.Add((Method: "JaroWinkler", Similarity: new JaroWinkler().GetSimilarity(text, textToCompare))); }
        if (options.Jaccard) { similarities.Add((Method: "Jaccard", Similarity: new JaccardSimilarity().GetSimilarity(text, textToCompare))); }
        return Math.Round(similarities.Average(x => x.Similarity) * 100, 2);
    }

    /// <summary>
    /// Calculates the similarity percentages between the input text and a list of strings.
    /// </summary>
    /// <param name="text">The input text.</param>
    /// <param name="listToCompare">The list of strings to compare with the input text.</param>
    /// <param name="options">Options for string similarity calculation. If null, default options are used.</param>
    /// <returns>A list of tuples containing the method used for comparison and the similarity percentage.</returns>
    public static List<(string Method, double Percentage)> GetStringListSimilarityPercentages(this string? text, List<string> listToCompare, GCScriptStringSimilarityOptions? options = null)
    {
        options ??= new GCScriptStringSimilarityOptions();
        if (options.ProcessText) { text = text.ProcessText(); listToCompare = listToCompare.Select(x => x.ProcessText()).Where(x => !x.IsNullOrWhiteSpace()).ToList(); }
        else { listToCompare = listToCompare.Where(x => !x.IsNullOrWhiteSpace()).ToList(); }

        if (text.IsNullOrWhiteSpace() || listToCompare.Count == 0) { return new(); }
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
