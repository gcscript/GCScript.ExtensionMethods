using GCScript.ExtensionMethods.Enums;
using GCScript.ExtensionMethods.Models;
using SimMetrics.Net.Metric;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace GCScript.ExtensionMethods;

public static class GCScriptStringExtensions {
	private static readonly char[] _invalidUrlChars = [' ', ',', '"', '\'', '\\', '<', '>', '[', ']', '{', '}', '|', '^', '`'];

	public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? text) => string.IsNullOrWhiteSpace(text);

	/// <summary>
	/// [PT-BR] Processa um texto com base nas opções especificadas.
	/// [EN] Processes a given text based on specified options.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O texto de entrada a ser processado.
	/// [EN] The input text to be processed.
	/// </param>
	/// <param name="removeAccents">
	/// [PT-BR] Um valor booleano indicando se os acentos devem ser removidos do texto. O valor padrão é true.
	/// [EN] A boolean indicating whether to remove accents from the text. The default value is true.
	/// </param>
	/// <param name="textTrim">
	/// [PT-BR] Um valor do enum especificando as opções de remoção de espaços no texto. O valor padrão é ETextTrim.Trim.
	/// [EN] An enum specifying the trimming options for the text. The default value is ETextTrim.Trim.
	/// </param>
	/// <param name="textCase">
	/// [PT-BR] Um valor do enum especificando as opções de transformação de caixa do texto. O valor padrão é ETextCase.ToLower.
	/// [EN] An enum specifying the case transformation options for the text. The default value is ETextCase.ToLower.
	/// </param>
	/// <param name="textType">
	/// [PT-BR] Um valor do enum especificando o tipo de caracteres a serem mantidos no texto. O valor padrão é ETextType.None.
	/// [EN] An enum specifying the type of characters to keep in the text. The default value is ETextType.None.
	/// </param>
	/// <param name="removeSpaces">
	/// [PT-BR] Um valor do enum especificando as opções de remoção de espaços no texto. O valor padrão é ETextRemoveSpaces.Duplicate.
	/// [EN] An enum specifying the space removal options for the text. The default value is ETextRemoveSpaces.Duplicate.
	/// </param>
	/// <returns>
	/// [PT-BR] O texto processado com base nas opções especificadas.
	/// [EN] The processed text based on the specified options.
	/// </returns>
	public static string ProcessText(this string? text,
									 bool removeAccents = true,
									 ETextTrim textTrim = ETextTrim.Trim,
									 ETextCase textCase = ETextCase.ToLower,
									 ETextType textType = ETextType.None,
									 ETextRemoveSpaces removeSpaces = ETextRemoveSpaces.Duplicate) {
		if (text.IsNullOrWhiteSpace()) { return string.Empty; }

		if (removeAccents) { text = text.RemoveAccents(); }

		switch (textTrim) {
			case ETextTrim.Trim: { text = text.Trim(); break; }
			case ETextTrim.TrimStart: { text = text.TrimStart(); break; }
			case ETextTrim.TrimEnd: { text = text.TrimEnd(); break; }
		}

		switch (textCase) {
			case ETextCase.ToLower: { text = text.ToLowerInvariant(); break; }
			case ETextCase.ToUpper: { text = text.ToUpperInvariant(); break; }
			case ETextCase.ToTitleCase: { text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLowerInvariant()); break; }
		}

		switch (removeSpaces) {
			case ETextRemoveSpaces.Duplicate: { text = text.RemoveDuplicateSpaces(); break; }
			case ETextRemoveSpaces.All: { text = text.RemoveAllSpaces(); break; }
		}

		switch (textType) {
			case ETextType.OnlyLetters: { text = text.OnlyLetters(); break; }
			case ETextType.OnlyNumbers: { text = text.OnlyNumbers(); break; }
			case ETextType.OnlyLettersAndNumbers: { text = text.OnlyLettersAndNumbers(); break; }
			case ETextType.OnlyLettersAndNumbersAndSpaces: { text = text.OnlyLettersAndNumbersAndSpaces(); break; }
		}

		return text;
	}

	/// <summary>
	/// [PT-BR] Processa o texto com as mesmas opções padrão de ProcessText, exceto que a caixa do texto é sempre convertida para maiúsculas (ToUpper).
	/// [EN] Processes the text with the same default options as ProcessText, except the text case is always converted to uppercase (ToUpper).
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O texto de entrada a ser processado.
	/// [EN] The input text to be processed.
	/// </param>
	/// <param name="removeAccents">
	/// [PT-BR] Indica se os acentos devem ser removidos do texto. O valor padrão é true.
	/// [EN] Indicates whether accents should be removed from the text. The default value is true.
	/// </param>
	/// <param name="textTrim">
	/// [PT-BR] Um valor do enum especificando as opções de remoção de espaços no texto. O valor padrão é ETextTrim.Trim.
	/// [EN] An enum specifying the trimming options for the text. The default value is ETextTrim.Trim.
	/// </param>
	/// <param name="textType">
	/// [PT-BR] Um valor do enum especificando o tipo de caracteres a serem mantidos no texto. O valor padrão é ETextType.None.
	/// [EN] An enum specifying the type of characters to keep in the text. The default value is ETextType.None.
	/// </param>
	/// <param name="removeSpaces">
	/// [PT-BR] Um valor do enum especificando as opções de remoção de espaços no texto. O valor padrão é ETextRemoveSpaces.Duplicate.
	/// [EN] An enum specifying the space removal options for the text. The default value is ETextRemoveSpaces.Duplicate.
	/// </param>
	/// <returns>
	/// [PT-BR] O texto processado em maiúsculas com base nas opções especificadas.
	/// [EN] The processed text in uppercase based on the specified options.
	/// </returns>
	public static string ProcessTextToUpper(this string? text,
											bool removeAccents = true,
											ETextTrim textTrim = ETextTrim.Trim,
											ETextType textType = ETextType.None,
											ETextRemoveSpaces removeSpaces = ETextRemoveSpaces.Duplicate) {
		return text.ProcessText(removeAccents: removeAccents,
								textTrim: textTrim,
								textCase: ETextCase.ToUpper,
								textType: textType,
								removeSpaces: removeSpaces);
	}

	/// <summary>
	/// [PT-BR] Remove os acentos do texto de entrada.
	/// [EN] Removes accents from the input text.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O texto de entrada do qual os acentos serão removidos.
	/// [EN] The input text from which accents will be removed.
	/// </param>
	/// <returns>
	/// [PT-BR] O texto de entrada sem os acentos.
	/// [EN] The input text with accents removed.
	/// </returns>
	public static string RemoveAccents(this string? text) {
		if (text.IsNullOrWhiteSpace()) { return string.Empty; }
		StringBuilder sbReturn = new StringBuilder();
		foreach (char letter in text.Normalize(NormalizationForm.FormD)) {
			if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)
				sbReturn.Append(letter);
		}
		return sbReturn.ToString();
	}

	/// <summary>
	/// [PT-BR] Remove todos os espaços do texto de entrada.
	/// [EN] Removes all spaces from the input text.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O texto de entrada do qual os espaços serão removidos.
	/// [EN] The input text from which spaces will be removed.
	/// </param>
	/// <returns>
	/// [PT-BR] O texto de entrada sem nenhum espaço.
	/// [EN] The input text with all spaces removed.
	/// </returns>
	public static string RemoveAllSpaces(this string? text) {
		if (text.IsNullOrWhiteSpace()) { return string.Empty; }
		text = Regex.Replace(text, @"\s", string.Empty);
		return text;
	}

	/// <summary>
	/// [PT-BR] Remove espaços duplicados do texto de entrada.
	/// [EN] Removes duplicate spaces from the input text.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O texto de entrada do qual os espaços duplicados serão removidos.
	/// [EN] The input text from which duplicate spaces will be removed.
	/// </param>
	/// <returns>
	/// [PT-BR] O texto de entrada com os espaços duplicados removidos.
	/// [EN] The input text with duplicate spaces removed.
	/// </returns>
	public static string RemoveDuplicateSpaces(this string? text) {
		if (text.IsNullOrWhiteSpace()) { return string.Empty; }
		text = Regex.Replace(text, @"\s+", " ");
		return text;
	}

	/// <summary>
	/// [PT-BR] Remove todos os caracteres não alfabéticos do texto de entrada.
	/// [EN] Removes all non-alphabetic characters from the input text.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O texto de entrada do qual os caracteres não alfabéticos serão removidos.
	/// [EN] The input text from which non-alphabetic characters will be removed.
	/// </param>
	/// <returns>
	/// [PT-BR] O texto de entrada contendo apenas letras.
	/// [EN] The input text containing only letters.
	/// </returns>
	public static string OnlyLetters(this string? text) {
		if (text.IsNullOrWhiteSpace()) { return string.Empty; }
		text = Regex.Replace(text, @"[^a-zA-Z]", string.Empty);
		return text;
	}

	/// <summary>
	/// [PT-BR] Remove todos os caracteres não numéricos do texto de entrada.
	/// [EN] Removes all non-numeric characters from the input text.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O texto de entrada do qual os caracteres não numéricos serão removidos.
	/// [EN] The input text from which non-numeric characters will be removed.
	/// </param>
	/// <returns>
	/// [PT-BR] O texto de entrada contendo apenas números.
	/// [EN] The input text containing only numeric characters.
	/// </returns>
	public static string OnlyNumbers(this string? text) {
		if (text.IsNullOrWhiteSpace()) { return string.Empty; }
		text = Regex.Replace(text, @"[^0-9]", string.Empty);
		return text;
	}

	/// <summary>
	/// [PT-BR] Remove todos os caracteres não alfanuméricos do texto de entrada.
	/// [EN] Removes all non-alphanumeric characters from the input text.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O texto de entrada do qual os caracteres não alfanuméricos serão removidos.
	/// [EN] The input text from which non-alphanumeric characters will be removed.
	/// </param>
	/// <returns>
	/// [PT-BR] O texto de entrada contendo apenas letras e números.
	/// [EN] The input text containing only letters and numbers.
	/// </returns>
	public static string OnlyLettersAndNumbers(this string? text) {
		if (text.IsNullOrWhiteSpace()) { return string.Empty; }
		text = Regex.Replace(text, @"[^a-zA-Z0-9]", string.Empty);
		return text;
	}

	/// <summary>
	/// [PT-BR] Remove todos os caracteres que não sejam letras, números ou espaços do texto de entrada.
	/// [EN] Removes all characters that are not letters, numbers, or spaces from the input text.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O texto de entrada do qual os caracteres que não sejam letras, números ou espaços serão removidos.
	/// [EN] The input text from which characters that are not letters, numbers, or spaces will be removed.
	/// </param>
	/// <returns>
	/// [PT-BR] O texto de entrada contendo apenas letras, números e espaços.
	/// [EN] The input text containing only letters, numbers, and spaces.
	/// </returns>
	public static string OnlyLettersAndNumbersAndSpaces(this string? text) {
		if (text.IsNullOrWhiteSpace()) { return string.Empty; }
		text = Regex.Replace(text, @"[^a-zA-Z0-9\s]", string.Empty);
		return text;
	}

	/// <summary>
	/// [PT-BR] Retorna os primeiros caracteres de uma string com base no comprimento especificado.
	/// [EN] Returns the first characters of a string based on the specified length.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] A string de entrada da qual os primeiros caracteres serão retornados.
	/// [EN] The input string from which the first characters will be returned.
	/// </param>
	/// <param name="length">
	/// [PT-BR] O número de caracteres a serem retornados a partir do início da string.
	/// [EN] The number of characters to be returned from the start of the string.
	/// </param>
	/// <returns>
	/// [PT-BR] Os primeiros caracteres da string especificada, ou uma string vazia se a entrada for nula ou se o comprimento for negativo.
	/// [EN] The first characters of the specified string, or an empty string if the input is null or if the length is negative.
	/// </returns>
	public static string Left(this string? text, int length) {
		if (text.IsNullOrWhiteSpace() || length < 0) { return string.Empty; }
		if (length > text.Length) { return text; }
		return text[..length];
	}

	/// <summary>
	/// [PT-BR] Retorna os últimos caracteres de uma string com base no comprimento especificado.
	/// [EN] Returns the last characters of a string based on the specified length.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] A string de entrada da qual os últimos caracteres serão retornados.
	/// [EN] The input string from which the last characters will be returned.
	/// </param>
	/// <param name="length">
	/// [PT-BR] O número de caracteres a serem retornados a partir do final da string.
	/// [EN] The number of characters to be returned from the end of the string.
	/// </param>
	/// <returns>
	/// [PT-BR] Os últimos caracteres da string especificada, ou uma string vazia se a entrada for nula ou se o comprimento for negativo.
	/// [EN] The last characters of the specified string, or an empty string if the input is null or if the length is negative.
	/// </returns>
	public static string Right(this string? text, int length) {
		if (text.IsNullOrWhiteSpace() || length < 0) { return string.Empty; }
		if (length > text.Length) { return text; }
		return text[^length..];
	}

	/// <summary>
	/// [PT-BR] Obtém as iniciais das palavras em uma string, considerando o comprimento mínimo de cada palavra e o comprimento mínimo do resultado. 
	/// Se as iniciais forem insuficientes, retorna os primeiros caracteres da string.
	/// [EN] Retrieves the initials of words in a string, considering the minimum length of each word and the minimum length of the result. 
	/// If the initials are insufficient, it returns the first characters of the string.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] A string de entrada da qual as iniciais serão obtidas.
	/// [EN] The input string from which the initials will be retrieved.
	/// </param>
	/// <param name="minWordLength">
	/// [PT-BR] Comprimento mínimo que uma palavra deve ter para que sua inicial seja incluída. O valor padrão é 3.
	/// [EN] Minimum length that a word must have for its initial to be included. The default value is 3.
	/// </param>
	/// <param name="minResultLength">
	/// [PT-BR] Comprimento mínimo que o resultado deve ter. O valor padrão é 2.
	/// [EN] Minimum length that the result must have. The default value is 2.
	/// </param>
	/// <param name="leftLength">
	/// [PT-BR] Número de caracteres a serem retornados do início da string caso as iniciais sejam insuficientes. O valor padrão é 3.
	/// [EN] Number of characters to be returned from the start of the string if the initials are insufficient. The default value is 3.
	/// </param>
	/// <returns>
	/// [PT-BR] As iniciais em letras maiúsculas das palavras que atendem ao critério de comprimento mínimo ou os primeiros caracteres da string se as iniciais forem insuficientes.
	/// [EN] The uppercase initials of the words that meet the minimum length criteria, or the first characters of the string if the initials are insufficient.
	/// </returns>
	public static string GetInitials(this string? text, int minWordLength = 3, int minResultLength = 2, int leftLength = 3) {
		text = text.ProcessText(textCase: ETextCase.ToUpper);
		if (text.IsNullOrWhiteSpace()) { return string.Empty; }
		minWordLength = Math.Max(1, minWordLength);
		minResultLength = Math.Max(1, minResultLength);
		string initials = string.Concat(text.Split(' ')
								.Where(word => !string.IsNullOrWhiteSpace(word) && word.Length >= minWordLength)
								.Select(word => word[0]));

		if (initials.Length < minResultLength) {
			return text.Left(leftLength);
		}

		return initials;
	}

	/// <summary>
	/// [PT-BR] Converte o texto de entrada para o formato slug.
	/// [EN] Converts the input text to slug format.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O texto de entrada a ser convertido.
	/// [EN] The input text to be converted.
	/// </param>
	/// <returns>
	/// [PT-BR] O texto de entrada convertido para o formato slug.
	/// [EN] The input text converted to slug format.
	/// </returns>
	public static string ToSlug(this string? text) {
		text = text.ProcessText(removeAccents: true, textTrim: ETextTrim.Trim, textCase: ETextCase.ToLower, textType: ETextType.OnlyLettersAndNumbersAndSpaces, removeSpaces: ETextRemoveSpaces.Duplicate);
		text = text.RemoveDuplicateSpaces();
		if (text.IsNullOrWhiteSpace()) { return string.Empty; }
		text = text.Replace(' ', '-');
		return text;
	}

	/// <summary>
	/// [PT-BR] Converte o texto de entrada para o formato snake_case.
	/// [EN] Converts the input text to snake_case format.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O texto de entrada a ser convertido.
	/// [EN] The input text to be converted.
	/// </param>
	/// <returns>
	/// [PT-BR] O texto de entrada convertido para o formato snake_case.
	/// [EN] The input text converted to snake_case format.
	/// </returns>
	public static string ToSnakeCase(this string? text) {
		text = text.ProcessText(removeAccents: true, textTrim: ETextTrim.Trim, textCase: ETextCase.ToLower, textType: ETextType.OnlyLettersAndNumbersAndSpaces, removeSpaces: ETextRemoveSpaces.Duplicate);
		text = text.RemoveDuplicateSpaces();
		if (text.IsNullOrWhiteSpace()) { return string.Empty; }
		text = text.Replace(' ', '_');
		return text;
	}

	/// <summary>
	/// [PT-BR] Converte o texto de entrada para o formato PascalCase.
	/// [EN] Converts the input text to PascalCase format.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O texto de entrada a ser convertido.
	/// [EN] The input text to be converted.
	/// </param>
	/// <returns>
	/// [PT-BR] O texto de entrada convertido para o formato PascalCase.
	/// [EN] The input text converted to PascalCase format.
	/// </returns>
	public static string ToPascalCase(this string? text) {
		text = text.ProcessText(removeAccents: true, textTrim: ETextTrim.Trim, textCase: ETextCase.ToTitleCase, textType: ETextType.OnlyLettersAndNumbers, removeSpaces: ETextRemoveSpaces.Duplicate);
		if (text.IsNullOrWhiteSpace()) { return string.Empty; }
		return text;
	}

	/// <summary>
	/// [PT-BR] Converte o texto de entrada para o formato camelCase.
	/// [EN] Converts the input text to camelCase format.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O texto de entrada a ser convertido.
	/// [EN] The input text to be converted.
	/// </param>
	/// <returns>
	/// [PT-BR] O texto de entrada convertido para o formato camelCase.
	/// [EN] The input text converted to camelCase format.
	/// </returns>
	public static string ToCamelCase(this string? text) {
		text = text.ProcessText(removeAccents: true, textTrim: ETextTrim.Trim, textCase: ETextCase.ToTitleCase, textType: ETextType.OnlyLettersAndNumbers, removeSpaces: ETextRemoveSpaces.Duplicate);
		if (text.IsNullOrWhiteSpace()) { return string.Empty; }
		if (text.Length > 1) { text = char.ToLowerInvariant(text[0]) + text[1..]; }
		return text;
	}

	/// <summary>
	/// [PT-BR] Converte o texto de entrada para o formato kebab-case.
	/// [EN] Converts the input text to kebab-case format.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O texto de entrada a ser convertido.
	/// [EN] The input text to be converted.
	/// </param>
	/// <returns>
	/// [PT-BR] O texto de entrada convertido para o formato kebab-case.
	/// [EN] The input text converted to kebab-case format.
	/// </returns>
	public static string ToKebabCase(this string? text) {
		text = text.ProcessText(removeAccents: false, textTrim: ETextTrim.Trim, textCase: ETextCase.ToLower, textType: ETextType.None, removeSpaces: ETextRemoveSpaces.Duplicate);
		text = text.RemoveDuplicateSpaces();
		if (text.IsNullOrWhiteSpace()) { return string.Empty; }
		text = text.Replace(' ', '-');
		return text;
	}

	/// <summary>
	/// [PT-BR] Inverte a ordem dos caracteres no texto fornecido.
	/// [EN] Reverses the order of characters in the given text.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O texto de entrada a ser invertido.
	/// [EN] The input text to be reversed.
	/// </param>
	/// <returns>
	/// [PT-BR] O texto com a ordem dos caracteres invertida.
	/// [EN] The text with the order of characters reversed.
	/// </returns>
	public static string Reverse(this string? text) {
		if (string.IsNullOrWhiteSpace(text)) return string.Empty;
		char[] chars = text.ToCharArray();
		Array.Reverse(chars);
		return new string(chars);
	}

	public static List<string> GetUniqueWords(this string text) {
		if (string.IsNullOrWhiteSpace(text)) return [];
		text = text.ProcessText(textCase: ETextCase.ToLower, textType: ETextType.OnlyLettersAndNumbersAndSpaces);
		return text.Split(' ').Distinct().ToList();
	}

	public static int WordCount(this string? text) {
		if (string.IsNullOrWhiteSpace(text)) return 0;
		text = text.ProcessText(textType: ETextType.OnlyLettersAndNumbersAndSpaces);
		return text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
	}

	public static string SafeSubstring(this string? text, int startIndex, int length) {
		if (string.IsNullOrWhiteSpace(text) || startIndex >= text.Length) return string.Empty;
		if (startIndex + length > text.Length) length = text.Length - startIndex;
		return text.Substring(startIndex, length);
	}


	/// <summary>
	/// [PT-BR] Conta o número de ocorrências de uma palavra especificada no texto fornecido.
	/// [EN] Counts the number of occurrences of a specified word in the given text.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O texto no qual as ocorrências serão procuradas.
	/// [EN] The text in which occurrences will be searched.
	/// </param>
	/// <param name="word">
	/// [PT-BR] A palavra cuja ocorrência será contada.
	/// [EN] The word whose occurrences will be counted.
	/// </param>
	/// <returns>
	/// [PT-BR] O número de ocorrências da palavra especificada no texto fornecido.
	/// [EN] The number of occurrences of the specified word in the given text.
	/// </returns>
	public static int CountOccurrences(this string text, string word) {
		int count = 0;
		int index = 0;
		while ((index = text.IndexOf(word, index)) != -1) {
			index += word.Length;
			count++;
		}
		return count;
	}

	/// <summary>
	/// [PT-BR] Conta o número de dígitos no texto fornecido.
	/// [EN] Counts the number of digits in the given text.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O texto de entrada no qual os dígitos serão contados.
	/// [EN] The input text in which digits will be counted.
	/// </param>
	/// <returns>
	/// [PT-BR] O número de dígitos encontrados no texto.
	/// [EN] The number of digits found in the text.
	/// </returns>
	public static int CountDigits(this string text) => text.Count(char.IsDigit);

	/// <summary>
	/// [PT-BR] Conta o número de letras no texto fornecido.
	/// [EN] Counts the number of letters in the given text.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O texto de entrada no qual as letras serão contadas.
	/// [EN] The input text in which letters will be counted.
	/// </param>
	/// <returns>
	/// [PT-BR] O número de letras encontradas no texto.
	/// [EN] The number of letters found in the text.
	/// </returns>
	public static int CountLetters(this string text) => text.Count(char.IsLetter);

	/// <summary>
	/// [PT-BR] Conta o número de letras e dígitos no texto fornecido.
	/// [EN] Counts the number of letters and digits in the given text.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O texto de entrada no qual as letras e dígitos serão contados.
	/// [EN] The input text in which letters and digits will be counted.
	/// </param>
	/// <returns>
	/// [PT-BR] O número de letras e dígitos encontrados no texto.
	/// [EN] The number of letters and digits found in the text.
	/// </returns>
	public static int CountLettersAndDigits(this string text) => text.Count(char.IsLetterOrDigit);

	/// <summary>
	/// [PT-BR] Conta o número de espaços no texto fornecido.
	/// [EN] Counts the number of spaces in the given text.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O texto de entrada no qual os espaços serão contados.
	/// [EN] The input text in which spaces will be counted.
	/// </param>
	/// <returns>
	/// [PT-BR] O número de espaços encontrados no texto.
	/// [EN] The number of spaces found in the text.
	/// </returns>
	public static int CountSpaces(this string text) => text.Count(char.IsWhiteSpace);

	/// <summary>
	/// [PT-BR] Conta o número de caracteres especiais no texto fornecido.
	/// [EN] Counts the number of special characters in the given text.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O texto de entrada no qual os caracteres especiais serão contados.
	/// [EN] The input text in which special characters will be counted.
	/// </param>
	/// <returns>
	/// [PT-BR] O número de caracteres especiais encontrados no texto.
	/// [EN] The number of special characters found in the text.
	/// </returns>
	public static int CountSpecialCharacters(this string text) => text.Count(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c));

	/// <summary>
	/// [PT-BR] Conta o número de ocorrências de um caractere específico no texto fornecido.
	/// [EN] Counts the number of occurrences of a specific character in the given text.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O texto de entrada no qual as ocorrências do caractere serão contadas.
	/// [EN] The input text in which the occurrences of the character will be counted.
	/// </param>
	/// <param name="character">
	/// [PT-BR] O caractere cuja ocorrência será contada.
	/// [EN] The character whose occurrences will be counted.
	/// </param>
	/// <returns>
	/// [PT-BR] O número de ocorrências do caractere especificado no texto.
	/// [EN] The number of occurrences of the specified character in the text.
	/// </returns>
	public static int CountCharOccurrences(this string text, char character) => text.Count(c => c == character);

	/// <summary>
	/// [PT-BR] Conta o número de letras maiúsculas no texto fornecido.
	/// [EN] Counts the number of uppercase letters in the given text.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O texto de entrada no qual as letras maiúsculas serão contadas.
	/// [EN] The input text in which uppercase letters will be counted.
	/// </param>
	/// <returns>
	/// [PT-BR] O número de letras maiúsculas encontradas no texto.
	/// [EN] The number of uppercase letters found in the text.
	/// </returns>
	public static int CountUppercaseLetters(this string text) => text.Count(char.IsUpper);

	/// <summary>
	/// [PT-BR] Conta o número de letras minúsculas no texto fornecido.
	/// [EN] Counts the number of lowercase letters in the given text.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O texto de entrada no qual as letras minúsculas serão contadas.
	/// [EN] The input text in which lowercase letters will be counted.
	/// </param>
	/// <returns>
	/// [PT-BR] O número de letras minúsculas encontradas no texto.
	/// [EN] The number of lowercase letters found in the text.
	/// </returns>
	public static int CountLowercaseLetters(this string text) => text.Count(char.IsLower);

	/// <summary>
	/// [PT-BR] Conta o número de caracteres de pontuação no texto fornecido.
	/// [EN] Counts the number of punctuation characters in the given text.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O texto de entrada no qual os caracteres de pontuação serão contados.
	/// [EN] The input text in which punctuation characters will be counted.
	/// </param>
	/// <returns>
	/// [PT-BR] O número de caracteres de pontuação encontrados no texto.
	/// [EN] The number of punctuation characters found in the text.
	/// </returns>
	public static int CountPunctuation(this string text) => text.Count(char.IsPunctuation);


	public static bool HasOnlyLetters(this string text) => text.All(char.IsLetter);


	public static bool HasOnlyNumbers(this string text) => text.All(char.IsDigit);


	public static bool HasOnlyLettersAndNumbers(this string text) => text.All(char.IsLetterOrDigit);


	public static bool HasOnlyLettersAndNumbersAndSpaces(this string text) => text.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c));


	public static bool StartsWithLetter(this string text) => !string.IsNullOrWhiteSpace(text) && char.IsLetter(text[0]);


	public static bool StartsWithNumber(this string text) => !string.IsNullOrWhiteSpace(text) && char.IsDigit(text[0]);


	public static bool StartsWithUppercase(this string text) => !string.IsNullOrWhiteSpace(text) && char.IsUpper(text[0]);


	public static bool StartsWithLowercase(this string text) => !string.IsNullOrWhiteSpace(text) && char.IsLower(text[0]);


	public static bool StartsWithSpecialCharacter(this string text) => !string.IsNullOrWhiteSpace(text) && !char.IsLetterOrDigit(text[0]) && !char.IsWhiteSpace(text[0]);

	/// <summary>
	/// [PT-BR] Verifica se o texto contém caracteres repetidos.
	/// [EN] Checks if the text contains repeated characters.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O texto de entrada a ser verificado.
	/// [EN] The input text to be checked.
	/// </param>
	/// <returns>
	/// [PT-BR] Retorna true se o texto contiver caracteres repetidos; caso contrário, false.
	/// [EN] Returns true if the text contains repeated characters; otherwise, false.
	/// </returns>

	public static bool HasRepeatedCharacters(this string text) {
		HashSet<char> seen = new HashSet<char>();
		foreach (var c in text) { if (!seen.Add(c)) return true; }
		return false;
	}

	/// <summary>
	/// [PT-BR] Alinha o texto especificado à esquerda preenchendo os espaços restantes com o caractere fornecido até atingir o comprimento especificado.
	/// [EN] Aligns the specified text to the left by filling the remaining spaces with the provided character until the specified length is reached.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O texto de entrada a ser alinhado à esquerda.
	/// [EN] The input text to be left-aligned.
	/// </param>
	/// <param name="fill">
	/// [PT-BR] O caractere usado para preencher os espaços restantes.
	/// [EN] The character used to fill the remaining spaces.
	/// </param>
	/// <param name="length">
	/// [PT-BR] O comprimento total desejado do texto alinhado à esquerda.
	/// [EN] The total desired length of the left-aligned text.
	/// </param>
	/// <returns>
	/// [PT-BR] O texto alinhado à esquerda com o comprimento especificado, preenchido com o caractere fornecido.
	/// [EN] The left-aligned text with the specified length, filled with the provided character.
	/// </returns>
	public static string AlignLeft(this string? text, char fill, int length) {
		if (text.IsNullOrWhiteSpace()) { return string.Empty; }
		if (length < 1) { return text; }
		return text.PadRight(length, fill);
	}

	/// <summary>
	/// [PT-BR] Alinha o texto especificado à direita preenchendo os espaços restantes com o caractere fornecido até atingir o comprimento especificado.
	/// [EN] Aligns the specified text to the right by filling the remaining spaces with the provided character until the specified length is reached.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O texto de entrada a ser alinhado à direita.
	/// [EN] The input text to be right-aligned.
	/// </param>
	/// <param name="fill">
	/// [PT-BR] O caractere usado para preencher os espaços restantes.
	/// [EN] The character used to fill the remaining spaces.
	/// </param>
	/// <param name="length">
	/// [PT-BR] O comprimento total desejado do texto alinhado à direita.
	/// [EN] The total desired length of the right-aligned text.
	/// </param>
	/// <returns>
	/// [PT-BR] O texto alinhado à direita com o comprimento especificado, preenchido com o caractere fornecido.
	/// [EN] The right-aligned text with the specified length, filled with the provided character.
	/// </returns>
	public static string AlignRight(this string? text, char fill, int length) {
		if (text.IsNullOrWhiteSpace()) { return string.Empty; }
		if (length < 1) { return text; }
		return text.PadLeft(length, fill);
	}

	/// <summary>
	/// [PT-BR] Centraliza o texto especificado preenchendo os espaços restantes com o caractere fornecido até atingir o comprimento especificado.
	/// [EN] Centers the specified text by filling the remaining spaces with the provided character until the specified length is reached.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O texto de entrada a ser centralizado.
	/// [EN] The input text to be centered.
	/// </param>
	/// <param name="fill">
	/// [PT-BR] O caractere usado para preencher os espaços restantes.
	/// [EN] The character used to fill the remaining spaces.
	/// </param>
	/// <param name="length">
	/// [PT-BR] O comprimento total desejado do texto centralizado.
	/// [EN] The total desired length of the centered text.
	/// </param>
	/// <returns>
	/// [PT-BR] O texto centralizado com o comprimento especificado, preenchido com o caractere fornecido.
	/// [EN] The centered text with the specified length, filled with the provided character.
	/// </returns>
	public static string AlignCenter(this string? text, char fill, int length) {
		if (text.IsNullOrWhiteSpace()) { return string.Empty; }
		if (length < 1) { return text; }
		if (text.Length >= length) { return text; }
		int spaces = length - text.Length;
		int leftSpaces = spaces / 2;
		text = text.PadLeft(text.Length + leftSpaces, fill);
		return text.PadRight(length, fill);
	}

	public static bool IsValidEmail(this string? email) {
		if (email.IsNullOrWhiteSpace()) { return false; }
		try {
			MailAddress addr = new System.Net.Mail.MailAddress(email);
			return addr.Address == email;
		}
		catch {
			return false;
		}
	}

	public static bool IsValidUrl(this string? url) {
		url = url?.Trim();
		if (string.IsNullOrWhiteSpace(url)) {
			return false;
		}

		if (url.IndexOfAny(_invalidUrlChars) >= 0) {
			return false;
		}

		if (Uri.TryCreate(url, UriKind.Absolute, out Uri? result)) {
			if (!(result.Scheme == Uri.UriSchemeHttp)) {
				return result.Scheme == Uri.UriSchemeHttps;
			}

			return true;
		}

		return false;
	}

	/// <summary>
	/// [PT-BR] Valida um CPF brasileiro (módulo 11, rejeita dígitos repetidos). Aceita com ou sem máscara.
	/// [EN] Validate a Brazilian CPF (modulo 11, rejects identical digits). Accepts masked or unmasked input.
	/// </summary>
	public static bool IsValidCpf(this string? value) {
		if (string.IsNullOrWhiteSpace(value)) { return false; }
		value = value.OnlyNumbers().PadLeft(11, '0');
		if (value == "00000000000") { return false; }

		Span<int> digits = stackalloc int[11];
		int count = 0;
		bool identical = true;
		int first = -1;
		foreach (var c in value) {
			if (!char.IsDigit(c)) { continue; }
			if (count >= 11) { return false; }
			int d = c - '0';
			if (first < 0) { first = d; }
			else if (d != first) { identical = false; }
			digits[count++] = d;
		}
		if (count != 11 || identical) { return false; }

		int sum1 = 0, sum2 = 0;
		for (int i = 0; i < 9; i++) {
			sum1 += digits[i] * (10 - i);
			sum2 += digits[i] * (11 - i);
		}
		int dv1 = sum1 % 11; dv1 = dv1 < 2 ? 0 : 11 - dv1;
		if (digits[9] != dv1) { return false; }
		sum2 += dv1 * 2;
		int dv2 = sum2 % 11; dv2 = dv2 < 2 ? 0 : 11 - dv2;
		return digits[10] == dv2;
	}

	/// <summary>
	/// [PT-BR] Valida um CNPJ brasileiro (módulo 11 com pesos, rejeita dígitos repetidos). Aceita com ou sem máscara.
	/// [EN] Validate a Brazilian CNPJ (weighted modulo 11). Accepts masked or unmasked input.
	/// </summary>
	public static bool IsValidCnpj(this string? value) {
		if (string.IsNullOrWhiteSpace(value)) { return false; }
		value = value.OnlyNumbers().PadLeft(14, '0');
		if (value == "00000000000000") { return false; }

		ReadOnlySpan<byte> m1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
		ReadOnlySpan<byte> m2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

		Span<int> digits = stackalloc int[14];
		int count = 0;
		bool identical = true;
		int first = -1;
		foreach (var c in value) {
			if (!char.IsDigit(c)) { continue; }
			if (count >= 14) { return false; }
			int d = c - '0';
			if (first < 0) { first = d; }
			else if (d != first) { identical = false; }
			digits[count++] = d;
		}
		if (count != 14 || identical) { return false; }

		int sum1 = 0, sum2 = 0;
		for (int i = 0; i < 12; i++) {
			sum1 += digits[i] * m1[i];
			sum2 += digits[i] * m2[i];
		}
		int dv1 = sum1 % 11; dv1 = dv1 < 2 ? 0 : 11 - dv1;
		if (digits[12] != dv1) { return false; }
		sum2 += dv1 * m2[12];
		int dv2 = sum2 % 11; dv2 = dv2 < 2 ? 0 : 11 - dv2;
		return digits[13] == dv2;
	}

	/// <summary>
	/// [PT-BR] Formata a string como CPF sem máscara, retornando apenas os 11 dígitos (preenche com zeros à esquerda se necessário).
	/// [EN] Formats the string as an unmasked CPF, returning only the 11 digits (pads with leading zeros if needed).
	/// </summary>
	public static string ToCpf(this string? value) => FormatCpf(value, withMask: false);

	/// <summary>
	/// [PT-BR] Formata a string como CPF com máscara no formato ###.###.###-## (preenche com zeros à esquerda se necessário).
	/// [EN] Formats the string as a masked CPF in the format ###.###.###-## (pads with leading zeros if needed).
	/// </summary>
	public static string ToCpfWithMask(this string? value) => FormatCpf(value, withMask: true);

	private static string FormatCpf(string? value, bool withMask) {
		if (string.IsNullOrWhiteSpace(value)) { return string.Empty; }
		value = value.OnlyNumbers().PadLeft(11, '0');
		if (value == "00000000000") { return string.Empty; }

		Span<char> digits = stackalloc char[11];
		int count = 0;
		foreach (var c in value) {
			if (!char.IsDigit(c)) { continue; }
			if (count >= 11) { return string.Empty; }
			digits[count++] = c;
		}
		if (count == 0) { return string.Empty; }
		if (count < 11) {
			int pad = 11 - count;
			digits[..count].CopyTo(digits[pad..]);
			digits[..pad].Fill('0');
		}

		if (!withMask) { return new string(digits); }
		return $"{digits[..3]}.{digits.Slice(3, 3)}.{digits.Slice(6, 3)}-{digits.Slice(9, 2)}";
	}

	/// <summary>
	/// [PT-BR] Formata a string como CNPJ sem máscara, retornando apenas os 14 dígitos (preenche com zeros à esquerda se necessário).
	/// [EN] Formats the string as an unmasked CNPJ, returning only the 14 digits (pads with leading zeros if needed).
	/// </summary>
	public static string ToCnpj(this string? value) => FormatCnpj(value, withMask: false);

	/// <summary>
	/// [PT-BR] Formata a string como CNPJ com máscara no formato ##.###.###/####-## (preenche com zeros à esquerda se necessário).
	/// [EN] Formats the string as a masked CNPJ in the format ##.###.###/####-## (pads with leading zeros if needed).
	/// </summary>
	public static string ToCnpjWithMask(this string? value) => FormatCnpj(value, withMask: true);

	private static string FormatCnpj(string? value, bool withMask) {
		if (string.IsNullOrWhiteSpace(value)) { return string.Empty; }
		value = value.OnlyNumbers().PadLeft(14, '0');
		if (value == "00000000000000") { return string.Empty; }

		Span<char> digits = stackalloc char[14];
		int count = 0;
		foreach (var c in value) {
			if (!char.IsDigit(c)) { continue; }
			if (count >= 14) { return string.Empty; }
			digits[count++] = c;
		}
		if (count == 0) { return string.Empty; }
		if (count < 14) {
			int pad = 14 - count;
			digits[..count].CopyTo(digits[pad..]);
			digits[..pad].Fill('0');
		}

		if (!withMask) { return new string(digits); }
		return $"{digits[..2]}.{digits.Slice(2, 3)}.{digits.Slice(5, 3)}/{digits.Slice(8, 4)}-{digits.Slice(12, 2)}";
	}

	public static ProcessNumber? ToProcessNumber(this string? processNumber) {
		try {
			if (string.IsNullOrWhiteSpace(processNumber)) { return null; }
			processNumber = processNumber.OnlyNumbers().PadLeft(20, '0');
			if (processNumber.Length != 20) { return null; }
			if (processNumber.All(x => x == '0')) { return null; }

			var number = processNumber[..7];
			var digit = processNumber[7..9];
			var year = processNumber[9..13];
			var agency = processNumber[13..14];
			var court = processNumber[14..16];
			var forum = processNumber[16..20];
			var fullFormatted = Regex.Replace(processNumber, "([0-9]{7})([0-9]{2})([0-9]{4})([0-9]{1})([0-9]{2})([0-9]{4})", "$1-$2.$3.$4.$5.$6");

			if (!int.TryParse(year, out int yearInt) || yearInt < 1900 || yearInt > DateTime.Now.Year + 1) { return null; }

			HashSet<string> validCourts = new HashSet<string>{
					"4.01", // AC | AM | AP | BA | DF | GO | MA | MT | PA | PI | RO | RR | TO
					"4.02", // ES | RJ
					"4.03", // MS | SP
					"4.04", // PR | RS | SC
					"4.05", // AL | CE | PB | PE | RN | SE
					"4.06", // MG
					"5.01", // RJ
					"5.02", // SP
					"5.03", // MG
					"5.04", // RS
					"5.05", // BA
					"5.06", // PE
					"5.07", // CE
					"5.08", // AP | PA
					"5.09", // PR
					"5.10", // DF | TO
					"5.11", // AM | RR
					"5.12", // SC
					"5.13", // PB
					"5.14", // AC | RO
					"5.15", // SP
					"5.16", // MA
					"5.17", // ES
					"5.18", // GO
					"5.19", // AL
					"5.20", // SE
					"5.21", // RN
					"5.22", // PI
					"5.23", // MT
					"5.24", // MS
					"8.01", // AC
					"8.02", // AL
					"8.03", // AP
					"8.04", // AM
					"8.05", // BA
					"8.06", // CE
					"8.07", // DF
					"8.08", // ES
					"8.09", // GO
					"8.10", // MA
					"8.11", // MT
					"8.12", // MS
					"8.13", // MG
					"8.14", // PA
					"8.15", // PB
					"8.16", // PR
					"8.17", // PE
					"8.18", // PI
					"8.19", // RJ
					"8.20", // RN
					"8.21", // RS
					"8.22", // RO
					"8.23", // RR
					"8.24", // SC
					"8.25", // SP
					"8.26", // SE
					"8.27", // TO
				};
			if (!validCourts.Contains($"{agency}.{court}")) { return null; }

			return new ProcessNumber() {
				Number = number,
				Digit = digit,
				Year = year,
				Agency = agency,
				Court = court,
				Forum = forum,
				FullFormatted = fullFormatted,
				FullUnformatted = processNumber
			};
		}
		catch { return null; }
	}

	/// <summary>
	/// [PT-BR] Verifica se o texto especificado contém as palavras especificadas com o número de ocorrências especificado, com base no tipo de comparação fornecido.
	/// [EN] Checks if the specified text contains the specified words with the specified number of occurrences based on the provided comparison type.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O texto no qual as ocorrências serão verificadas.
	/// [EN] The text in which occurrences will be checked.
	/// </param>
	/// <param name="words">
	/// [PT-BR] O array de palavras a serem verificadas no texto.
	/// [EN] The array of words to be checked in the text.
	/// </param>
	/// <param name="occurrences">
	/// [PT-BR] O número esperado de ocorrências para cada palavra.
	/// [EN] The expected number of occurrences for each word.
	/// </param>
	/// <param name="comparisonType">
	/// [PT-BR] O tipo de comparação a ser usado para verificar as ocorrências. O valor padrão é EOccurrenceComparisonOperator.Equals.
	/// [EN] The type of comparison to use for checking occurrences. The default value is EOccurrenceComparisonOperator.Equals.
	/// </param>
	/// <returns>
	/// [PT-BR] Retorna true se o texto contiver as palavras especificadas com o número de ocorrências especificado com base no tipo de comparação; caso contrário, false.
	/// [EN] Returns true if the text contains the specified words with the specified number of occurrences based on the comparison type; otherwise, false.
	/// </returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// [PT-BR] Lançada quando um tipo de comparação inválido é fornecido.
	/// [EN] Thrown when an invalid comparison type is provided.
	/// </exception>
	public static bool CheckOccurrences(this string text, string[] words, int occurrences, EOccurrenceComparisonOperator comparisonType = EOccurrenceComparisonOperator.Equals) {
		foreach (string word in words) {
			int count = 0;
			int index = 0;
			while ((index = text.IndexOf(word, index)) != -1) {
				index += word.Length;
				count++;
			}

			switch (comparisonType) {
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
	/// [PT-BR] Calcula a porcentagem de similaridade entre dois textos de entrada.
	/// [EN] Calculates the similarity percentage between two input texts.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O primeiro texto de entrada.
	/// [EN] The first input text.
	/// </param>
	/// <param name="textToCompare">
	/// [PT-BR] O segundo texto de entrada a ser comparado com o primeiro.
	/// [EN] The second input text to compare with the first.
	/// </param>
	/// <param name="options">
	/// [PT-BR] Opções para o cálculo de similaridade de strings. Se nulo, as opções padrão serão usadas.
	/// [EN] Options for string similarity calculation. If null, default options will be used.
	/// </param>
	/// <returns>
	/// [PT-BR] A porcentagem de similaridade entre os dois textos de entrada.
	/// [EN] The similarity percentage between the two input texts.
	/// </returns>
	public static double GetStringSimilarityPercentage(this string? text, string? textToCompare, GCScriptStringSimilarityOptions? options = null) {
		options ??= new GCScriptStringSimilarityOptions() { Levenstein = true, JaroWinkler = false, Jaccard = false, ProcessText = true };
		if (options.ProcessText) { text = text.ProcessText(); textToCompare = textToCompare.ProcessText(); }
		if (text.IsNullOrWhiteSpace() || textToCompare.IsNullOrWhiteSpace()) { return 0; }
		List<(string Method, double Similarity)> similarities = new();
		if (options.Levenstein) { similarities.Add((Method: "Levenstein", Similarity: new Levenstein().GetSimilarity(text, textToCompare))); }
		if (options.JaroWinkler) { similarities.Add((Method: "JaroWinkler", Similarity: new JaroWinkler().GetSimilarity(text, textToCompare))); }
		if (options.Jaccard) { similarities.Add((Method: "Jaccard", Similarity: new JaccardSimilarity().GetSimilarity(text, textToCompare))); }
		return Math.Round(similarities.Average(x => x.Similarity) * 100, 2);
	}

	/// <summary>
	/// [PT-BR] Calcula as porcentagens de similaridade entre o texto de entrada e uma lista de strings.
	/// [EN] Calculates the similarity percentages between the input text and a list of strings.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O texto de entrada.
	/// [EN] The input text.
	/// </param>
	/// <param name="listToCompare">
	/// [PT-BR] A lista de strings a ser comparada com o texto de entrada.
	/// [EN] The list of strings to compare with the input text.
	/// </param>
	/// <param name="options">
	/// [PT-BR] Opções para o cálculo de similaridade de strings. Se nulo, as opções padrão serão usadas.
	/// [EN] Options for string similarity calculation. If null, default options will be used.
	/// </param>
	/// <returns>
	/// [PT-BR] Uma lista de tuplas contendo o método usado para a comparação e a porcentagem de similaridade.
	/// [EN] A list of tuples containing the method used for comparison and the similarity percentage.
	/// </returns>
	public static List<(string Method, double Percentage)> GetStringListSimilarityPercentages(this string? text, List<string> listToCompare, GCScriptStringSimilarityOptions? options = null) {
		options ??= new GCScriptStringSimilarityOptions() { Levenstein = true, JaroWinkler = false, Jaccard = false, ProcessText = true };
		if (options.ProcessText) { text = text.ProcessText(); listToCompare = listToCompare.Select(x => x.ProcessText()).Where(x => !x.IsNullOrWhiteSpace()).ToList(); }
		else { listToCompare = listToCompare.Where(x => !x.IsNullOrWhiteSpace()).ToList(); }

		if (text.IsNullOrWhiteSpace() || listToCompare.Count == 0) { return new(); }
		List<(string Method, double Percentage)> similarities = new();
		foreach (string item in listToCompare) {
			List<(string Method, double Similarity)> similaritiesItem = new();
			if (options.Levenstein) { similaritiesItem.Add((Method: "Levenstein", Similarity: new Levenstein().GetSimilarity(text, item))); }
			if (options.JaroWinkler) { similaritiesItem.Add((Method: "JaroWinkler", Similarity: new JaroWinkler().GetSimilarity(text, item))); }
			if (options.Jaccard) { similaritiesItem.Add((Method: "Jaccard", Similarity: new JaccardSimilarity().GetSimilarity(text, item))); }
			similarities.Add((Method: item, Percentage: Math.Round(similaritiesItem.Average(x => x.Similarity) * 100, 2)));
		}
		return [.. similarities.OrderByDescending(x => x.Percentage).ThenBy(x => x.Method).ToArray()];
	}
}
