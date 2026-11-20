using GCScript.ExtensionMethods.Enums;

namespace GCScript.ExtensionMethods;

/// <summary>
/// [PT-BR] Unidades Federativas: reconhecer a UF escrita em texto livre e obter o nome do estado.
/// [EN] Federative Units: recognize the UF written as free text and get the state name.
/// </summary>
/// <remarks>
/// [PT-BR] A UF chega de cadastros e planilhas como sigla ("RJ", " rj ") ou como nome ("Rio de Janeiro", "SAO PAULO"). O reconhecimento compara sem diferenciar maiúsculas, acentos e espaços repetidos, mas sempre por igualdade com uma sigla ou um nome conhecido: "Rio" ou "Janeiro" não viram RJ.
/// [EN] The UF arrives from registries and spreadsheets as an abbreviation ("RJ", " rj ") or as a name ("Rio de Janeiro", "SAO PAULO"). Recognition ignores case, accents and repeated spaces, but always by equality with a known abbreviation or name: "Rio" or "Janeiro" do not become RJ.
/// </remarks>
public static class GCScriptUFExtensions {
	private static readonly Dictionary<EUF, string> StateNames = new() {
		[EUF.AC] = "Acre",
		[EUF.AL] = "Alagoas",
		[EUF.AP] = "Amapá",
		[EUF.AM] = "Amazonas",
		[EUF.BA] = "Bahia",
		[EUF.CE] = "Ceará",
		[EUF.DF] = "Distrito Federal",
		[EUF.ES] = "Espírito Santo",
		[EUF.GO] = "Goiás",
		[EUF.MA] = "Maranhão",
		[EUF.MT] = "Mato Grosso",
		[EUF.MS] = "Mato Grosso do Sul",
		[EUF.MG] = "Minas Gerais",
		[EUF.PA] = "Pará",
		[EUF.PB] = "Paraíba",
		[EUF.PR] = "Paraná",
		[EUF.PE] = "Pernambuco",
		[EUF.PI] = "Piauí",
		[EUF.RJ] = "Rio de Janeiro",
		[EUF.RN] = "Rio Grande do Norte",
		[EUF.RS] = "Rio Grande do Sul",
		[EUF.RO] = "Rondônia",
		[EUF.RR] = "Roraima",
		[EUF.SC] = "Santa Catarina",
		[EUF.SP] = "São Paulo",
		[EUF.SE] = "Sergipe",
		[EUF.TO] = "Tocantins"
	};

	/// <summary>
	/// [PT-BR] Siglas e nomes já normalizados (maiúsculas, sem acento), apontando para a UF.
	/// [EN] Already normalized abbreviations and names (uppercase, no accents), pointing to the UF.
	/// </summary>
	/// <remarks>
	/// [PT-BR] Montado a partir das siglas e dos nomes, e não por Enum.TryParse: este aceitaria "1" ou "19" como UF.
	/// [EN] Built from the abbreviations and names, not with Enum.TryParse: that would accept "1" or "19" as a UF.
	/// </remarks>
	private static readonly Dictionary<string, EUF> Recognition = StateNames
		.SelectMany(item => new[] {
			new KeyValuePair<string, EUF>(item.Key.ToString(), item.Key),
			new KeyValuePair<string, EUF>(item.Value.ProcessTextToUpper(), item.Key)
		})
		.ToDictionary(item => item.Key, item => item.Value, StringComparer.Ordinal);

	/// <summary>
	/// [PT-BR] Verifica se o texto é uma UF: a sigla ou o nome do estado.
	/// [EN] Checks whether the text is a UF: the abbreviation or the state name.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O texto da UF, como veio do cadastro.
	/// [EN] The UF text, as it came from the registry.
	/// </param>
	/// <returns>
	/// [PT-BR] true se o texto for uma UF reconhecida; caso contrário, false.
	/// [EN] true if the text is a recognized UF; otherwise, false.
	/// </returns>
	public static bool IsValidUF(this string? text) => text.TryToUF(out _);

	/// <summary>
	/// [PT-BR] Tenta converter a sigla ou o nome do estado em uma EUF.
	/// [EN] Tries to convert the abbreviation or the state name into an EUF.
	/// </summary>
	/// <param name="text">
	/// [PT-BR] O texto da UF, como veio do cadastro. Maiúsculas, acentos e espaços repetidos não importam.
	/// [EN] The UF text, as it came from the registry. Case, accents and repeated spaces do not matter.
	/// </param>
	/// <param name="uf">
	/// [PT-BR] A UF reconhecida, ou o valor padrão (que não é UF) quando o texto não é reconhecido.
	/// [EN] The recognized UF, or the default value (which is not a UF) when the text is not recognized.
	/// </param>
	/// <returns>
	/// [PT-BR] true se o texto foi reconhecido; caso contrário, false.
	/// [EN] true if the text was recognized; otherwise, false.
	/// </returns>
	public static bool TryToUF(this string? text, out EUF uf) {
		uf = default;
		string processed = text.ProcessTextToUpper();
		if (processed.Length == 0) { return false; }
		return Recognition.TryGetValue(processed, out uf);
	}

	/// <summary>
	/// [PT-BR] O nome do estado, com acentos, pronto para exibir (ex.: RJ → "Rio de Janeiro").
	/// [EN] The state name, with accents, ready for display (e.g.: RJ → "Rio de Janeiro").
	/// </summary>
	/// <param name="uf">
	/// [PT-BR] A UF.
	/// [EN] The UF.
	/// </param>
	/// <returns>
	/// [PT-BR] O nome do estado.
	/// [EN] The state name.
	/// </returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// [PT-BR] Quando o valor não é uma UF (ex.: o valor padrão 0).
	/// [EN] When the value is not a UF (e.g.: the default value 0).
	/// </exception>
	public static string GetStateName(this EUF uf)
		=> StateNames.TryGetValue(uf, out string? name)
			? name
			: throw new ArgumentOutOfRangeException(nameof(uf), uf, "O valor não é uma UF.");
}
