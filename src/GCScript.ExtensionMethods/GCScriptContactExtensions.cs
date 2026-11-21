namespace GCScript.ExtensionMethods;

/// <summary>
/// [PT-BR] Validação e formatação de CEP e de telefone brasileiro (DDD + número), aceitando o texto com ou sem máscara.
/// [EN] Validation and formatting of Brazilian postal codes (CEP) and phone numbers (area code + number), with or without mask.
/// </summary>
public static class GCScriptContactExtensions {
	// DDDs em uso no Brasil (Anatel).
	private static readonly HashSet<int> _ddds = [
		11, 12, 13, 14, 15, 16, 17, 18, 19,
		21, 22, 24, 27, 28,
		31, 32, 33, 34, 35, 37, 38,
		41, 42, 43, 44, 45, 46, 47, 48, 49,
		51, 53, 54, 55,
		61, 62, 63, 64, 65, 66, 67, 68, 69,
		71, 73, 74, 75, 77, 79,
		81, 82, 83, 84, 85, 86, 87, 88, 89,
		91, 92, 93, 94, 95, 96, 97, 98, 99,
	];

	/// <summary>
	/// [PT-BR] Indica se o texto é um CEP: exatamente 8 dígitos, com ou sem máscara (20040-020, 20.040-020), e não todo zero.
	/// [EN] Indicates whether the text is a CEP: exactly 8 digits, with or without mask, and not all zeros.
	/// </summary>
	public static bool IsValidCep(this string? value) {
		var digits = CepDigits(value);
		return digits.Length == 8 && digits != "00000000";
	}

	/// <summary>
	/// [PT-BR] O CEP só com os 8 dígitos; vazio se não for um CEP válido.
	/// [EN] The CEP with only its 8 digits; empty if it is not a valid CEP.
	/// </summary>
	public static string ToCep(this string? value)
		=> value.IsValidCep() ? CepDigits(value) : string.Empty;

	/// <summary>
	/// [PT-BR] O CEP com máscara no formato #####-###; vazio se não for um CEP válido.
	/// [EN] The CEP masked as #####-###; empty if it is not a valid CEP.
	/// </summary>
	public static string ToCepWithMask(this string? value) {
		var cep = value.ToCep();
		return cep.Length == 0 ? string.Empty : $"{cep.Substring(0, 5)}-{cep.Substring(5)}";
	}

	/// <summary>
	/// [PT-BR] Indica se o texto é um telefone brasileiro: DDD em uso e celular (9 dígitos começando com 9) ou fixo
	/// (8 dígitos começando de 2 a 5). Aceita máscara e o código do país (55) na frente.
	/// [EN] Indicates whether the text is a Brazilian phone: valid area code and mobile (9 digits starting with 9) or
	/// landline (8 digits starting with 2 to 5). Accepts a mask and the country code (55) in front.
	/// </summary>
	public static bool IsValidBrazilianPhone(this string? value)
		=> PhoneDigits(value).Length > 0;

	/// <summary>
	/// [PT-BR] O telefone só com os dígitos do DDD e do número (10 ou 11), sem o código do país; vazio se não for válido.
	/// [EN] The phone with only the area code and number digits (10 or 11), without the country code; empty if invalid.
	/// </summary>
	public static string ToBrazilianPhone(this string? value)
		=> PhoneDigits(value);

	/// <summary>
	/// [PT-BR] O telefone com máscara: (21) 99999-9999 para celular e (21) 3333-4444 para fixo; vazio se não for válido.
	/// [EN] The masked phone: (21) 99999-9999 for mobile and (21) 3333-4444 for landline; empty if invalid.
	/// </summary>
	public static string ToBrazilianPhoneWithMask(this string? value) {
		var phone = PhoneDigits(value);
		if (phone.Length == 0) { return string.Empty; }

		var ddd = phone.Substring(0, 2);
		var number = phone.Substring(2);
		var split = number.Length - 4;
		return $"({ddd}) {number.Substring(0, split)}-{number.Substring(split)}";
	}

	private static string CepDigits(string? value) {
		if (string.IsNullOrWhiteSpace(value)) { return string.Empty; }

		// Só dígitos e os separadores da máscara: letras no meio não são um CEP.
		foreach (var c in value!.Trim()) {
			if (!char.IsDigit(c) && c != '-' && c != '.' && c != ' ') { return string.Empty; }
		}

		return value.OnlyNumbers();
	}

	private static string PhoneDigits(string? value) {
		if (string.IsNullOrWhiteSpace(value)) { return string.Empty; }

		foreach (var c in value!.Trim()) {
			if (!char.IsDigit(c) && c != '(' && c != ')' && c != '-' && c != ' ' && c != '+' && c != '.') { return string.Empty; }
		}

		var digits = value.OnlyNumbers();
		if (digits.Length is 12 or 13 && digits.StartsWith("55", StringComparison.Ordinal)) {
			digits = digits.Substring(2);
		}

		if (digits.Length is not (10 or 11)) { return string.Empty; }
		if (!_ddds.Contains(int.Parse(digits.Substring(0, 2), System.Globalization.CultureInfo.InvariantCulture))) { return string.Empty; }

		var first = digits[2];
		var mobile = digits.Length == 11 && first == '9';
		var landline = digits.Length == 10 && first is >= '2' and <= '5';
		return mobile || landline ? digits : string.Empty;
	}
}
