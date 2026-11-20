namespace GCScript.ExtensionMethods.Enums;

/// <summary>
/// [PT-BR] Unidades Federativas do Brasil (os 26 estados e o Distrito Federal), pela sigla.
/// [EN] Brazilian Federative Units (the 26 states and the Federal District), by abbreviation.
/// </summary>
/// <remarks>
/// [PT-BR] Começa em 1 de propósito: o valor padrão (0) não é uma UF, então um campo esquecido não vira "AC" em silêncio. Guarde e transmita pelo nome (a sigla), não pelo número.
/// [EN] Starts at 1 on purpose: the default value (0) is not a UF, so a forgotten field does not silently become "AC". Store and transmit it by name (the abbreviation), not by number.
/// </remarks>
public enum EUF {
	AC = 1,
	AL,
	AP,
	AM,
	BA,
	CE,
	DF,
	ES,
	GO,
	MA,
	MT,
	MS,
	MG,
	PA,
	PB,
	PR,
	PE,
	PI,
	RJ,
	RN,
	RS,
	RO,
	RR,
	SC,
	SP,
	SE,
	TO
}
