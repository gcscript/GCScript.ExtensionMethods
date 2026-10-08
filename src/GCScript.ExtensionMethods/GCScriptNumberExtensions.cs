using System.Globalization;
using System.Text.RegularExpressions;

namespace GCScript.ExtensionMethods;

/// <summary>
/// [PT-BR] Números escritos como texto no padrão brasileiro.
/// [EN] Numbers written as text in the Brazilian format.
/// </summary>
public static class GCScriptNumberExtensions {
	/// <summary>
	/// [PT-BR] Vírgula decimal e ponto de milhar só em grupos de três dígitos. [0-9] e não \d: o \d do .NET aceita dígitos de outros alfabetos.
	/// [EN] Decimal comma and thousands dot only in groups of three digits. [0-9] and not \d: .NET's \d accepts digits from other scripts.
	/// </summary>
	private static readonly Regex BrazilianNumberPattern = new(@"^[+-]?(?:[0-9]{1,3}(?:\.[0-9]{3})+|[0-9]+)?(?:,[0-9]+)?$", RegexOptions.CultureInvariant);

	/// <summary>
	/// [PT-BR] Lê um número escrito no padrão brasileiro: vírgula decimal e ponto de milhar ("1.234,56", "1234,56", "1.234", "-0,5", ",5").
	/// [EN] Reads a number written in the Brazilian format: decimal comma and thousands dot ("1.234,56", "1234,56", "1.234", "-0,5", ",5").
	/// </summary>
	/// <remarks>
	/// [PT-BR] Estrito de propósito: o ponto só é aceito como separador de milhar em grupos de três dígitos. "10.5" e "1.50" são ambíguos - podem ter vindo de um sistema em inglês - e o decimal.Parse em pt-BR os leria como 105 e 150, sem aviso. Aqui eles não são reconhecidos, para quem chama decidir o que fazer.
	/// Espaços nas pontas (inclusive o espaço rígido, comum em textos copiados da web) são ignorados. Símbolo de moeda, porcentagem e espaço entre os dígitos não são aceitos.
	/// [EN] Strict on purpose: the dot is only accepted as a thousands separator in groups of three digits. "10.5" and "1.50" are ambiguous - they may come from a system in English - and decimal.Parse in pt-BR would read them as 105 and 150, silently. Here they are not recognized, so the caller decides what to do.
	/// Leading and trailing spaces (including the non-breaking space, common in text copied from the web) are ignored. Currency symbols, percent signs and spaces between digits are not accepted.
	/// </remarks>
	/// <param name="text">
	/// [PT-BR] O texto com o número.
	/// [EN] The text with the number.
	/// </param>
	/// <param name="number">
	/// [PT-BR] O número lido, ou 0 quando o texto não é um número no padrão brasileiro.
	/// [EN] The number read, or 0 when the text is not a number in the Brazilian format.
	/// </param>
	/// <returns>
	/// [PT-BR] true se o texto é um número no padrão brasileiro que cabe em decimal; caso contrário, false.
	/// [EN] true if the text is a number in the Brazilian format that fits in a decimal; otherwise, false.
	/// </returns>
	public static bool TryParseBrazilianNumber(this string? text, out decimal number) {
		number = 0;
		if (text.IsNullOrWhiteSpace()) { return false; }

		string trimmed = text.Trim();
		if (!trimmed.Any(c => c >= '0' && c <= '9') || !BrazilianNumberPattern.IsMatch(trimmed)) { return false; }

		// Já validado: sem os pontos de milhar e com ponto decimal, o texto é lido na cultura invariante
		string invariant = trimmed.Replace(".", string.Empty).Replace(',', '.');
		return decimal.TryParse(invariant, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out number);
	}
}
