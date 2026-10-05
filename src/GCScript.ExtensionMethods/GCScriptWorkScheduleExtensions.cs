using GCScript.ExtensionMethods.Enums;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace GCScript.ExtensionMethods;

/// <summary>
/// [PT-BR] Escalas de trabalho: como o nome da escala escrito em cadastro vira uma escala conhecida, e quantos dias ela rende num período.
/// [EN] Work schedules: how the schedule name written in a registry becomes a known schedule, and how many days it yields in a period.
/// </summary>
/// <remarks>
/// [PT-BR] A escala chega como texto livre digitado por terceiros - "6x1", "06/01", "44h", "seg qua sex" são todas escalas reais do mesmo cadastro. Por isso o caminho é sempre o mesmo: ToWorkSchedule reduz o texto a um dos títulos de WorkScheduleTitles, e só títulos entram na contagem de dias.
/// [EN] The schedule arrives as free text typed by third parties - "6x1", "06/01", "44h", "seg qua sex" are all real schedules from the same registry. Hence the path is always the same: ToWorkSchedule reduces the text to one of the WorkScheduleTitles, and only titles enter the day count.
/// </remarks>
public static class GCScriptWorkScheduleExtensions {
	/// <summary>
	/// [PT-BR] Escala assumida quando o texto vem vazio ou não é reconhecido.
	/// [EN] Schedule assumed when the text is empty or not recognized.
	/// </summary>
	/// <remarks>
	/// [PT-BR] Não é descarte: 6x1 é a escala da maioria absoluta dos cadastros, e recusar a linha por causa de um rótulo mal escrito tiraria do cálculo alguém que trabalha.
	/// [EN] It is not a discard: 6x1 is the schedule of the vast majority of registries, and rejecting the row because of a misspelled label would leave out someone who works.
	/// </remarks>
	public const string DefaultWorkSchedule = "6X1";

	/// <summary>
	/// [PT-BR] Como cada escala distribui os dias trabalhados.
	/// [EN] How each schedule distributes the worked days.
	/// </summary>
	/// <remarks>
	/// [PT-BR] Duas naturezas diferentes, e por isso os dois campos: escala de comércio se define por dias da semana (5x2 é segunda a sexta, não "5 dias seguidos"), e escala de plantão se define por ciclo - trabalha N, folga M, ignorando o calendário.
	/// Escala em horas vira ciclo em dias pela soma: 24x48 são 72 horas, três dias, um de trabalho. Quando a soma não fecha em dias inteiros, o ciclo é a menor quantidade de dias que fecha - 12x48 são 60 horas, dois plantões a cada cinco dias -, o que acerta a contagem no período, embora não a posição exata de cada plantão.
	/// [EN] Two different natures, hence the two fields: a retail schedule is defined by weekdays (5x2 is Monday to Friday, not "5 days in a row"), and a shift schedule is defined by a cycle - work N, rest M, ignoring the calendar.
	/// An hour-based schedule becomes a day cycle by its sum: 24x48 is 72 hours, three days, one of them worked. When the sum does not close in whole days, the cycle is the smallest number of days that does - 12x48 is 60 hours, two shifts every five days -, which gets the count in the period right, though not the exact position of each shift.
	/// </remarks>
	private sealed record Rhythm(DayOfWeek[]? Weekdays, int Work, int Rest) {
		public static Rhythm Cycle(int work, int rest) => new(null, work, rest);

		public static Rhythm Week(params DayOfWeek[] days) => new(days, 0, 0);
	}

	private static readonly Dictionary<string, Rhythm> Catalog = new(StringComparer.Ordinal) {
		["5X2"] = Rhythm.Week(DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday),
		["6X1"] = Rhythm.Week(DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday),
		["6X2"] = Rhythm.Cycle(6, 2),

		["12X36"] = Rhythm.Cycle(1, 1),
		["12X60"] = Rhythm.Cycle(1, 2),
		["24X72"] = Rhythm.Cycle(1, 3),
		["24X120"] = Rhythm.Cycle(1, 5),
		["24X48"] = Rhythm.Cycle(1, 2),
		["12X48"] = Rhythm.Cycle(2, 3),

		["7X7"] = Rhythm.Cycle(7, 7),
		["14X14"] = Rhythm.Cycle(14, 14),
		["14X21"] = Rhythm.Cycle(14, 21),

		["1X6"] = Rhythm.Cycle(1, 6),
		["2X5"] = Rhythm.Cycle(2, 5),
		["3X4"] = Rhythm.Cycle(3, 4),
		["4X3"] = Rhythm.Cycle(4, 3),

		["2X1"] = Rhythm.Cycle(2, 1),
		["3X1"] = Rhythm.Cycle(3, 1),
		["4X1"] = Rhythm.Cycle(4, 1),
		["4X2"] = Rhythm.Cycle(4, 2),
		["5X1"] = Rhythm.Cycle(5, 1),

		["TER..QUI"] = Rhythm.Week(DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday),
		["QUA..SAB"] = Rhythm.Week(DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday),
		["SEG|QUA|SEX"] = Rhythm.Week(DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday),
		["TER|QUI|SAB"] = Rhythm.Week(DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Saturday),
		["QUA|QUI"] = Rhythm.Week(DayOfWeek.Wednesday, DayOfWeek.Thursday),
		["TER|QUI"] = Rhythm.Week(DayOfWeek.Tuesday, DayOfWeek.Thursday)
	};

	/// <summary>
	/// [PT-BR] Reconhecimento do rótulo, na ordem em que é avaliado - e a ordem é parte da regra.
	/// [EN] Label recognition, in evaluation order - and the order is part of the rule.
	/// </summary>
	/// <remarks>
	/// [PT-BR] O ponto dos padrões casa qualquer separador, que é o que faz "6X1", "6/1" e "6-1" caírem na mesma escala sem uma entrada por grafia. Como ponto também casa dígito, um mesmo rótulo pode satisfazer mais de um padrão, e aí vence quem vier primeiro. As disputas reais: "24X120" e "24X144" também satisfazem "4X1"; "14X14" também satisfaz "4X1", e "14X21" satisfaz "4X2" e "2X1"; "TERAQUI" também satisfaz "TER|QUI"; "TERQUISAB" idem. Em todas o padrão mais específico está acima, e mover qualquer um deles para baixo do concorrente muda o resultado. "7X7" fica por último de propósito: "7.7" é curto e genérico, e lá embaixo só pega o que nenhum outro padrão reconheceu.
	/// [EN] The dot in the patterns matches any separator, which is what makes "6X1", "6/1" and "6-1" land on the same schedule without one entry per spelling. Since the dot also matches a digit, one label may satisfy more than one pattern, and then the first one wins. The real contests: "24X120" and "24X144" also satisfy "4X1"; "14X14" also satisfies "4X1", and "14X21" satisfies "4X2" and "2X1"; "TERAQUI" also satisfies "TER|QUI"; "TERQUISAB" likewise. In all of them the more specific pattern is above, and moving any of them below its competitor changes the result. "7X7" is last on purpose: "7.7" is short and generic, and down there it only takes what no other pattern recognized.
	/// </remarks>
	private static readonly (Regex Pattern, string Title)[] Recognition =
	[
		(new Regex("06.01|6.1"), "6X1"),
		(new Regex("06.02|6.2"), "6X2"),
		(new Regex("05.02|5.2|44H|40H"), "5X2"),
		(new Regex("12.36|12.84"), "12X36"),
		(new Regex("12.60|13.60"), "12X60"),
		(new Regex("24.72"), "24X72"),
		(new Regex("24.120|24.121|24.144"), "24X120"),
		(new Regex("24.48"), "24X48"),
		(new Regex("12.48"), "12X48"),
		(new Regex("14.14"), "14X14"),
		(new Regex("14.21"), "14X21"),
		(new Regex("SEG.?QUA.?SEX"), "SEG|QUA|SEX"),
		(new Regex("SEG(UNDA)?(.?FEIRA)?.?(ATE|A)?.?SEX"), "5X2"),
		(new Regex("SEG(UNDA)?(.?FEIRA)?.?(ATE|A)?.?SAB"), "6X1"),
		(new Regex("TER.?QUI.?SAB"), "TER|QUI|SAB"),
		(new Regex("TERAQUI"), "TER..QUI"),
		(new Regex("QUAASAB"), "QUA..SAB"),
		(new Regex("TER.?QUI"), "TER|QUI"),
		(new Regex("QUA.?QUI"), "QUA|QUI"),
		(new Regex("01.06|1.6"), "1X6"),
		(new Regex("02.05|2.5"), "2X5"),
		(new Regex("03.04|3.4"), "3X4"),
		(new Regex("04.03|4.3"), "4X3"),
		(new Regex("02.01|2.1"), "2X1"),
		(new Regex("04.01|4.1"), "4X1"),
		(new Regex("04.02|4.2"), "4X2"),
		(new Regex("03.01|3.1"), "3X1"),
		(new Regex("05.01|5.1"), "5X1"),
		(new Regex("07.07|7.7"), "7X7")
	];

	/// <summary>
	/// [PT-BR] Escalas conhecidas, que são as únicas que ToWorkSchedule devolve.
	/// [EN] Known schedules, which are the only ones ToWorkSchedule returns.
	/// </summary>
	public static IReadOnlyCollection<string> WorkScheduleTitles => Catalog.Keys;

	/// <summary>
	/// [PT-BR] Converte o texto livre de uma escala de trabalho em um dos títulos de WorkScheduleTitles.
	/// [EN] Converts the free text of a work schedule into one of the WorkScheduleTitles.
	/// </summary>
	/// <remarks>
	/// [PT-BR] "SEG QUA SEX" e "SEGQUASEX" são a mesma escala, então todo espaço é retirado antes da comparação, que é feita em maiúsculas e sem acento: os padrões são escritos assim.
	/// [EN] "SEG QUA SEX" and "SEGQUASEX" are the same schedule, so every space is removed before matching, which is done in uppercase and without accents: the patterns are written that way.
	/// </remarks>
	/// <param name="text">
	/// [PT-BR] O texto da escala, como veio do cadastro.
	/// [EN] The schedule text, as it came from the registry.
	/// </param>
	/// <returns>
	/// [PT-BR] O título da escala reconhecida, ou DefaultWorkSchedule quando o texto vem vazio ou não é reconhecido.
	/// [EN] The recognized schedule title, or DefaultWorkSchedule when the text is empty or not recognized.
	/// </returns>
	public static string ToWorkSchedule(this string? text) => text.TryToWorkSchedule(out string? schedule) ? schedule : DefaultWorkSchedule;

	/// <summary>
	/// [PT-BR] Tenta converter o texto livre de uma escala de trabalho em um dos títulos de WorkScheduleTitles, sem cair na escala padrão.
	/// [EN] Tries to convert the free text of a work schedule into one of the WorkScheduleTitles, without falling back to the default schedule.
	/// </summary>
	/// <remarks>
	/// [PT-BR] ToWorkSchedule devolve DefaultWorkSchedule tanto para "6x1" quanto para "administrativo", e quem chama não tem como separar os dois casos. Este método existe para isso: auditar o cadastro, avisar o usuário ou recusar a linha é decisão de quem chama, e ela só é possível se o reconhecimento disser quando falhou.
	/// [EN] ToWorkSchedule returns DefaultWorkSchedule both for "6x1" and for "administrativo", and the caller cannot tell the two apart. This method exists for that: auditing the registry, warning the user or rejecting the row is the caller's decision, and it is only possible if recognition says when it failed.
	/// </remarks>
	/// <param name="text">
	/// [PT-BR] O texto da escala, como veio do cadastro.
	/// [EN] The schedule text, as it came from the registry.
	/// </param>
	/// <param name="schedule">
	/// [PT-BR] O título da escala reconhecida, ou nulo quando o texto vem vazio ou não é reconhecido.
	/// [EN] The recognized schedule title, or null when the text is empty or not recognized.
	/// </param>
	/// <returns>
	/// [PT-BR] true se o texto foi reconhecido; caso contrário, false.
	/// [EN] true if the text was recognized; otherwise, false.
	/// </returns>
	public static bool TryToWorkSchedule(this string? text, [NotNullWhen(true)] out string? schedule) {
		string processed = text.ProcessTextToUpper(removeSpaces: ETextRemoveSpaces.All);
		schedule = null;
		if (processed.Length == 0) { return false; }

		// "TER..QUI" e "QUA..SAB" não casam com os próprios padrões; sem isto, converter um título já convertido o trocaria pela escala padrão.
		if (Catalog.ContainsKey(processed)) {
			schedule = processed;
			return true;
		}

		foreach (var (pattern, title) in Recognition) {
			if (pattern.IsMatch(processed)) {
				schedule = title;
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// [PT-BR] Quantos dias a escala rende entre as duas datas, ambas incluídas.
	/// [EN] How many days the schedule yields between the two dates, both inclusive.
	/// </summary>
	/// <remarks>
	/// [PT-BR] O texto passa por ToWorkSchedule antes, então aceita a mesma grafia livre e cai na escala padrão quando não reconhece. Para saber se caiu, chame TryToWorkSchedule antes.
	/// [EN] The text goes through ToWorkSchedule first, so it accepts the same free spelling and falls back to the default schedule when it is not recognized. To know whether it fell back, call TryToWorkSchedule first.
	/// </remarks>
	/// <param name="text">
	/// [PT-BR] O texto da escala, como veio do cadastro.
	/// [EN] The schedule text, as it came from the registry.
	/// </param>
	/// <param name="start">
	/// [PT-BR] A data inicial do período.
	/// [EN] The start date of the period.
	/// </param>
	/// <param name="end">
	/// [PT-BR] A data final do período. Não pode ser anterior à inicial.
	/// [EN] The end date of the period. Cannot be earlier than the start date.
	/// </param>
	/// <returns>
	/// [PT-BR] A quantidade de dias trabalhados no período.
	/// [EN] The number of worked days in the period.
	/// </returns>
	public static int GetWorkDays(this string? text, DateTime start, DateTime end) {
		EnsurePeriod(start, end);
		return CountDays(Catalog[text.ToWorkSchedule()], start, end);
	}

	/// <summary>
	/// [PT-BR] Quantos dias cada escala conhecida rende entre as duas datas, ambas incluídas.
	/// [EN] How many days each known schedule yields between the two dates, both inclusive.
	/// </summary>
	/// <remarks>
	/// [PT-BR] Útil quando milhares de registros compartilham o mesmo período: a tabela é montada uma vez, e cada registro só precisa do título já convertido para consultá-la.
	/// [EN] Useful when thousands of records share the same period: the table is built once, and each record only needs its already converted title to look it up.
	/// </remarks>
	/// <param name="start">
	/// [PT-BR] A data inicial do período.
	/// [EN] The start date of the period.
	/// </param>
	/// <param name="end">
	/// [PT-BR] A data final do período. Não pode ser anterior à inicial.
	/// [EN] The end date of the period. Cannot be earlier than the start date.
	/// </param>
	/// <returns>
	/// [PT-BR] Um dicionário com o título da escala como chave e a quantidade de dias como valor.
	/// [EN] A dictionary with the schedule title as key and the number of days as value.
	/// </returns>
	public static IReadOnlyDictionary<string, int> GetWorkScheduleDays(this DateTime start, DateTime end) {
		EnsurePeriod(start, end);
		return Catalog.ToDictionary(item => item.Key, item => CountDays(item.Value, start, end), StringComparer.Ordinal);
	}

	private static void EnsurePeriod(DateTime start, DateTime end) {
		if (end.Date < start.Date) {
			throw new ArgumentException("A data final não pode ser anterior à inicial.", nameof(end));
		}
	}

	/// <remarks>
	/// [PT-BR] Fórmula fechada em vez de andar dia a dia: o laço custava um passo por dia do período e estourava com ArgumentOutOfRangeException perto de DateTime.MaxValue, porque avançava a data além do último dia antes de testar o fim. Aqui nenhuma data é criada, só se conta. O resultado é comparado nos testes contra o laço de origem.
	/// [EN] Closed formula instead of walking day by day: the loop cost one step per day of the period and threw ArgumentOutOfRangeException near DateTime.MaxValue, because it moved the date past the last day before testing the end. Here no date is created, only counted. The result is compared in the tests against the original loop.
	/// </remarks>
	private static int CountDays(Rhythm rhythm, DateTime start, DateTime end) {
		// Cabe em int: o calendário inteiro tem cerca de 3,65 milhões de dias.
		int totalDays = (end.Date - start.Date).Days + 1;

		if (rhythm.Weekdays is { } weekdays) {
			int days = totalDays / 7 * weekdays.Length;
			int firstDayOfWeek = (int)start.DayOfWeek;
			for (int i = 0; i < totalDays % 7; i++) {
				if (Array.IndexOf(weekdays, (DayOfWeek)((firstDayOfWeek + i) % 7)) >= 0) { days++; }
			}
			return days;
		}

		// O ciclo começa trabalhando no primeiro dia: dentro de cada volta, as primeiras Work posições são de trabalho.
		int cycle = rhythm.Work + rhythm.Rest;
		return totalDays / cycle * rhythm.Work + Math.Min(totalDays % cycle, rhythm.Work);
	}
}
