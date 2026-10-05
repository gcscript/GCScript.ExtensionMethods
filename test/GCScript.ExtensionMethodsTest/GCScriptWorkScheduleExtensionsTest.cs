using GCScript.ExtensionMethods;
using GCScript.ExtensionMethods.Enums;
using System.Text.RegularExpressions;

namespace GCScript.ExtensionMethodsTest;

/// <summary>
/// Escalas de trabalho: reconhecer o rótulo e contar os dias do período.
/// </summary>
/// <remarks>
/// O código veio do GridFlow, que por sua vez partiu de um código de produção que já
/// rodava. Os dois blocos delicados - o reconhecimento e a contagem - são comparados
/// contra a versão literal daquele código sobre centenas de entradas: um teste de
/// exemplos escolhidos a dedo passaria mesmo com a ordem dos padrões trocada, que é
/// justamente o defeito mais fácil de introduzir aqui.
/// </remarks>
public class GCScriptWorkScheduleExtensionsTest
{
    /// <summary>
    /// Rótulos plausíveis de cadastro, mais a combinação mecânica de "N separador M"
    /// para cobrir o que os padrões disputam entre si.
    /// </summary>
    private static IReadOnlyList<string> Labels()
    {
        HashSet<string> labels = new(StringComparer.Ordinal)
        {
            "", "   ", "6x1", "6X1", "06/01", "06.01", "6-1", "6 x 1", "5x2", "05/02",
            "44h", "40H", "44 H", "12x36", "12/36", "12X84", "12x60", "13/60", "24x72",
            "24x120", "24/121", "24x144", "1x6", "2x5", "3x4", "4x3", "2x1", "3x1",
            "4x1", "4x2", "5x1", "seg qua sex", "SEG/QUA/SEX", "SegQuaSex",
            "ter qui sab", "TER-QUI-SAB", "ter a qui", "qua a sab", "ter qui", "TERQUI",
            "qua qui", "escala 6x1", "ESCALA 12X36", "administrativo", "xyz", "0", "-",
            "1", "12", "36", "sem escala", "6", "x", "Ter/Qui", "SEG QUA  SEX", "sáb",
            "6x2", "06/02", "24x48", "24/48", "12x48", "12 x 48", "14x14", "14/21", "7x7", "07/07",
            "segunda a sexta", "seg a sex", "Seg-Sex", "segunda-feira a sexta-feira", "seg à sex",
            "seg até sex", "segunda a sábado", "seg a sab", "SEG/SAB", "16x1", "26/10"
        };

        // Cobre a disputa entre padrões curtos e longos: "24x120" contém "2x1".
        foreach (var separator in new[] { "x", "X", "/", "-", ".", " " })
        {
            for (var work = 0; work <= 24; work++)
            {
                foreach (var rest in new[] { 0, 1, 2, 3, 4, 5, 6, 7, 14, 21, 36, 48, 60, 72, 120, 144 })
                {
                    labels.Add($"{work}{separator}{rest}");
                    labels.Add($"{work:00}{separator}{rest:00}");
                }
            }
        }

        return [.. labels];
    }

    /// <summary>
    /// Escalas acrescentadas depois da versão de origem, cada uma com o padrão que a
    /// reconhece. É a lista das únicas diferenças permitidas em relação à origem.
    /// </summary>
    private static readonly Regex NewRules = new("06.02|6.2|24.48|12.48|14.14|14.21|07.07|7.7|SEG(UNDA)?(.?FEIRA)?.?(ATE|A)?.?(SEX|SAB)");

    /// <summary>
    /// Fora das escalas acrescentadas depois, o reconhecimento precisa dar exatamente o
    /// mesmo resultado da versão de origem - inclusive nos casos em que dois padrões
    /// competem pelo mesmo texto. Toda divergência tem de ser explicada por uma regra
    /// nova; qualquer outra é regressão.
    /// </summary>
    [Fact(DisplayName = "ToWorkSchedule - Should only differ from the original where a new rule applies")]
    public void ToWorkSchedule_ShouldOnlyDifferFromOriginalWhereNewRuleApplies()
    {
        foreach (var label in Labels())
        {
            var result = label.ToWorkSchedule();
            var literal = ToWorkScheduleLiteral(label);
            var processed = label.ProcessText(textCase: ETextCase.ToUpper, removeSpaces: ETextRemoveSpaces.All);

            Assert.True(
                result == literal || NewRules.IsMatch(processed),
                $"Divergência no rótulo \"{label}\" sem regra nova que a explique: atual={result}, origem literal={literal}");
        }
    }

    /// <summary>
    /// As escalas acrescentadas depois da versão de origem. Os rótulos de 14x14 e 14x21
    /// caíam em 4X1 e 4X2 na origem, por conterem "4x1" e "4x2"; o 6x2 era contado como
    /// 6X1; os demais nem eram reconhecidos e caíam na escala padrão.
    /// </summary>
    [Theory(DisplayName = "ToWorkSchedule - Should recognize the schedules added after the original")]
    [InlineData("6x2", "6X2")]
    [InlineData("06/02", "6X2")]
    [InlineData("24x48", "24X48")]
    [InlineData("24/48", "24X48")]
    [InlineData("12x48", "12X48")]
    [InlineData("12 x 48", "12X48")]
    [InlineData("14x14", "14X14")]
    [InlineData("14/21", "14X21")]
    [InlineData("7x7", "7X7")]
    [InlineData("07/07", "7X7")]
    [InlineData("segunda a sexta", "5X2")]
    [InlineData("Segunda à Sexta", "5X2")]
    [InlineData("seg a sex", "5X2")]
    [InlineData("Seg-Sex", "5X2")]
    [InlineData("segunda-feira a sexta-feira", "5X2")]
    [InlineData("seg até sex", "5X2")]
    [InlineData("segunda a sábado", "6X1")]
    [InlineData("seg a sab", "6X1")]
    [InlineData("SEG/SAB", "6X1")]
    public void ToWorkSchedule_ShouldRecognizeAddedSchedules(string label, string expected)
    {
        Assert.True(label.TryToWorkSchedule(out var schedule), $"\"{label}\" não foi reconhecido.");
        Assert.Equal(expected, schedule);
    }

    /// <summary>
    /// Disputas entre padrões que a ordem precisa resolver: três dias fixos não podem
    /// virar faixa de segunda a sexta, nem uma escala longa ser engolida pela curta
    /// contida nela.
    /// </summary>
    [Theory(DisplayName = "ToWorkSchedule - Overlapping patterns should resolve by order")]
    [InlineData("seg qua sex", "SEG|QUA|SEX")]
    [InlineData("24x120", "24X120")]
    [InlineData("14x14", "14X14")]
    [InlineData("14x21", "14X21")]
    [InlineData("6x1", "6X1")]
    [InlineData("12x36", "12X36")]
    public void ToWorkSchedule_OverlappingPatterns_ShouldResolveByOrder(string label, string expected)
    {
        Assert.Equal(expected, label.ToWorkSchedule());
    }

    /// <summary>
    /// Falsos positivos herdados da origem, mantidos por decisão: o reconhecimento é por
    /// trecho do texto, então "16x1" e "26/10" contêm "6x1" e "6/1". Ficam fixados aqui
    /// para que mudar isso seja uma escolha, não um efeito colateral.
    /// </summary>
    [Theory(DisplayName = "ToWorkSchedule - Known inherited false positives")]
    [InlineData("16x1", "6X1")]
    [InlineData("26/10", "6X1")]
    public void ToWorkSchedule_KnownInheritedFalsePositives(string label, string expected)
    {
        Assert.Equal(expected, label.ToWorkSchedule());
    }

    /// <summary>
    /// Transcrição literal do <c>TreatWorkSchedule</c> de origem, preservada só dentro
    /// do teste. A única diferença deliberada é o ramo repetido de 24X120, que na
    /// origem aparecia duas vezes e era inalcançável na segunda.
    /// </summary>
    private static string ToWorkScheduleLiteral(string schedule)
    {
        schedule = schedule.ProcessText(textCase: ETextCase.ToUpper, removeSpaces: ETextRemoveSpaces.All) ?? string.Empty;

        if (string.IsNullOrWhiteSpace(schedule)) { return "6X1"; }
        if (Regex.IsMatch(schedule, "06.01|06.02|6.1|6.2")) { return "6X1"; }
        if (Regex.IsMatch(schedule, "05.02|5.2|44H|40H")) { return "5X2"; }
        if (Regex.IsMatch(schedule, "12.36|12.84")) { return "12X36"; }
        if (Regex.IsMatch(schedule, "12.60|13.60")) { return "12X60"; }
        if (Regex.IsMatch(schedule, "24.72")) { return "24X72"; }
        if (Regex.IsMatch(schedule, "24.120|24.121|24.144")) { return "24X120"; }
        if (Regex.IsMatch(schedule, "SEG.?QUA.?SEX")) { return "SEG|QUA|SEX"; }
        if (Regex.IsMatch(schedule, "TER.?QUI.?SAB")) { return "TER|QUI|SAB"; }
        if (Regex.IsMatch(schedule, "TERAQUI")) { return "TER..QUI"; }
        if (Regex.IsMatch(schedule, "QUAASAB")) { return "QUA..SAB"; }
        if (Regex.IsMatch(schedule, "TER.?QUI")) { return "TER|QUI"; }
        if (Regex.IsMatch(schedule, "QUA.?QUI")) { return "QUA|QUI"; }
        if (Regex.IsMatch(schedule, "01.06|1.6")) { return "1X6"; }
        if (Regex.IsMatch(schedule, "02.05|2.5")) { return "2X5"; }
        if (Regex.IsMatch(schedule, "03.04|3.4")) { return "3X4"; }
        if (Regex.IsMatch(schedule, "04.03|4.3")) { return "4X3"; }
        if (Regex.IsMatch(schedule, "02.01|2.1")) { return "2X1"; }
        if (Regex.IsMatch(schedule, "04.01|4.1")) { return "4X1"; }
        if (Regex.IsMatch(schedule, "04.02|4.2")) { return "4X2"; }
        if (Regex.IsMatch(schedule, "03.01|3.1")) { return "3X1"; }
        if (Regex.IsMatch(schedule, "05.01|5.1")) { return "5X1"; }

        return "6X1";
    }

    [Theory(DisplayName = "ToWorkSchedule - Should recognize common spellings")]
    [InlineData("6x1", "6X1")]
    [InlineData("06/01", "6X1")]
    [InlineData("44h", "5X2")]
    [InlineData("12 x 36", "12X36")]
    [InlineData("24/144", "24X120")]
    [InlineData("Seg Qua Sex", "SEG|QUA|SEX")]
    [InlineData("ter a qui", "TER..QUI")]
    [InlineData("TER-QUI-SÁB", "TER|QUI|SAB")]
    public void ToWorkSchedule_ShouldRecognizeCommonSpellings(string label, string expected)
    {
        Assert.Equal(expected, label.ToWorkSchedule());
    }

    /// <summary>
    /// Invariante estrutural: todo título que o reconhecimento devolve tem de existir
    /// no catálogo de dias. Se não existisse, a contagem estouraria com
    /// <see cref="KeyNotFoundException"/> só quando alguém tivesse aquela escala no
    /// cadastro - e é justamente a escala rara.
    /// </summary>
    [Fact(DisplayName = "ToWorkSchedule - Every recognized title should exist in the catalog")]
    public void ToWorkSchedule_EveryTitleShouldExistInCatalog()
    {
        foreach (var label in Labels())
        {
            Assert.Contains(label.ToWorkSchedule(), GCScriptWorkScheduleExtensions.WorkScheduleTitles);
        }
    }

    /// <summary>
    /// Converter um título já convertido não pode mudá-lo. Na origem "TER..QUI" e
    /// "QUA..SAB" não casavam com os próprios padrões e voltavam como 6X1.
    /// </summary>
    [Fact(DisplayName = "ToWorkSchedule - Should be idempotent")]
    public void ToWorkSchedule_ShouldBeIdempotent()
    {
        foreach (var title in GCScriptWorkScheduleExtensions.WorkScheduleTitles)
        {
            Assert.Equal(title, title.ToWorkSchedule());
        }
    }

    [Theory(DisplayName = "ToWorkSchedule - Blank or unknown should fall back to the default")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("administrativo")]
    public void ToWorkSchedule_BlankOrUnknown_ShouldReturnDefault(string? label)
    {
        Assert.Equal(GCScriptWorkScheduleExtensions.DefaultWorkSchedule, label.ToWorkSchedule());
    }

    /// <summary>
    /// A contagem de dias precisa bater com a de origem em toda escala e todo período.
    /// </summary>
    /// <remarks>
    /// Um início por dia da semana, de propósito: escala definida por dias da semana
    /// muda de resultado conforme onde o período começa, e um único período de teste
    /// esconderia exatamente esse erro.
    /// </remarks>
    [Fact(DisplayName = "GetWorkScheduleDays - Should match the literal original version")]
    public void GetWorkScheduleDays_ShouldMatchLiteralOriginalVersion()
    {
        for (var offset = 0; offset < 7; offset++)
        {
            var start = new DateTime(2026, 1, 1).AddDays(offset);

            foreach (var duration in new[] { 0, 1, 6, 7, 13, 29, 30, 44, 89 })
            {
                var end = start.AddDays(duration);
                var result = start.GetWorkScheduleDays(end);
                var literal = DaysLiteral(start, end);

                Assert.Equal(literal.Count, result.Count);

                foreach (var pair in literal)
                {
                    Assert.True(
                        pair.Value == result[pair.Key],
                        $"Divergência em {pair.Key} de {start:dd/MM/yyyy} a {end:dd/MM/yyyy}: atual={result[pair.Key]}, origem literal={pair.Value}");
                }
            }
        }
    }

    /// <summary>
    /// Transcrição literal do <c>CalculateSchedules</c> de origem, com os mesmos
    /// parâmetros de ciclo e os mesmos dias da semana.
    /// </summary>
    private static Dictionary<string, int> DaysLiteral(DateTime start, DateTime end) => new()
    {
        ["5X2"] = ByWeekday(start, end, false, true, true, true, true, true, false),
        ["6X1"] = ByWeekday(start, end, false, true, true, true, true, true, true),
        ["12X36"] = ByCycle(start, end, 1, 1),
        ["12X60"] = ByCycle(start, end, 1, 2),
        ["24X72"] = ByCycle(start, end, 1, 3),
        ["24X120"] = ByCycle(start, end, 1, 5),
        ["1X6"] = ByCycle(start, end, 1, 6),
        ["2X5"] = ByCycle(start, end, 2, 5),
        ["3X4"] = ByCycle(start, end, 3, 4),
        ["4X3"] = ByCycle(start, end, 4, 3),
        ["2X1"] = ByCycle(start, end, 2, 1),
        ["4X1"] = ByCycle(start, end, 4, 1),
        ["4X2"] = ByCycle(start, end, 4, 2),
        ["TER..QUI"] = ByWeekday(start, end, false, false, true, true, true, false, false),
        ["QUA..SAB"] = ByWeekday(start, end, false, false, false, true, true, true, true),
        ["SEG|QUA|SEX"] = ByWeekday(start, end, false, true, false, true, false, true, false),
        ["TER|QUI|SAB"] = ByWeekday(start, end, false, false, true, false, true, false, true),
        ["QUA|QUI"] = ByWeekday(start, end, false, false, false, true, true, false, false),
        ["TER|QUI"] = ByWeekday(start, end, false, false, true, false, true, false, false),
        ["3X1"] = ByCycle(start, end, 3, 1),
        ["5X1"] = ByCycle(start, end, 5, 1),

        // Acrescentadas depois da versão de origem, contadas pelo mesmo laço dela.
        ["6X2"] = ByCycle(start, end, 6, 2),
        ["24X48"] = ByCycle(start, end, 1, 2),
        ["12X48"] = ByCycle(start, end, 2, 3),
        ["7X7"] = ByCycle(start, end, 7, 7),
        ["14X14"] = ByCycle(start, end, 14, 14),
        ["14X21"] = ByCycle(start, end, 14, 21)
    };

    private static int ByCycle(DateTime start, DateTime end, int work, int rest)
    {
        var days = 0;
        var day = start.Date;
        var remaining = work;

        while (day <= end.Date)
        {
            days++;
            remaining--;
            day = day.AddDays(1);

            if (remaining == 0)
            {
                remaining = work;
                day = day.AddDays(rest);
            }
        }

        return days;
    }

    private static int ByWeekday(DateTime start, DateTime end, bool sunday, bool monday, bool tuesday, bool wednesday, bool thursday, bool friday, bool saturday)
    {
        var days = 0;
        var day = start.Date;

        while (day <= end.Date)
        {
            var counts = day.DayOfWeek switch
            {
                DayOfWeek.Sunday => sunday,
                DayOfWeek.Monday => monday,
                DayOfWeek.Tuesday => tuesday,
                DayOfWeek.Wednesday => wednesday,
                DayOfWeek.Thursday => thursday,
                DayOfWeek.Friday => friday,
                _ => saturday
            };

            if (counts) { days++; }

            day = day.AddDays(1);
        }

        return days;
    }

    /// <summary>
    /// Uma semana cheia, começando na segunda. Fixa o significado que a comparação com
    /// a versão literal sozinha não fixaria - as duas poderiam estar erradas do mesmo jeito.
    /// </summary>
    [Fact(DisplayName = "GetWorkScheduleDays - A full week should yield the schedule days")]
    public void GetWorkScheduleDays_FullWeek_ShouldYieldScheduleDays()
    {
        var monday = new DateTime(2026, 1, 5);
        var days = monday.GetWorkScheduleDays(monday.AddDays(6));

        Assert.Equal(6, days["6X1"]);
        Assert.Equal(5, days["5X2"]);
        Assert.Equal(4, days["12X36"]);
        Assert.Equal(3, days["SEG|QUA|SEX"]);
        Assert.Equal(1, days["1X6"]);
    }

    /// <summary>
    /// Um mês de 30 dias nas escalas acrescentadas. 12x48 rende 12 plantões, e não 10:
    /// o ciclo é de 60 horas, dois plantões a cada cinco dias.
    /// </summary>
    [Fact(DisplayName = "GetWorkScheduleDays - Added schedules over a 30-day month")]
    public void GetWorkScheduleDays_AddedSchedules_ThirtyDayMonth()
    {
        var days = new DateTime(2026, 4, 1).GetWorkScheduleDays(new DateTime(2026, 4, 30));

        Assert.Equal(24, days["6X2"]);
        Assert.Equal(10, days["24X48"]);
        Assert.Equal(12, days["12X48"]);
        Assert.Equal(16, days["7X7"]);
        Assert.Equal(16, days["14X14"]);
        Assert.Equal(14, days["14X21"]);
    }

    [Fact(DisplayName = "GetWorkScheduleDays - A single-day period should count that day")]
    public void GetWorkScheduleDays_SingleDay_ShouldCountThatDay()
    {
        var days = new DateTime(2026, 1, 5).GetWorkScheduleDays(new DateTime(2026, 1, 5));

        Assert.Equal(1, days["6X1"]);
        Assert.Equal(1, days["12X36"]);
    }

    [Fact(DisplayName = "GetWorkScheduleDays - An inverted period should be rejected")]
    public void GetWorkScheduleDays_InvertedPeriod_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => new DateTime(2026, 2, 1).GetWorkScheduleDays(new DateTime(2026, 1, 1)));
    }

    [Theory(DisplayName = "GetWorkDays - Should recognize the label and count its days")]
    [InlineData("6x1", 6)]
    [InlineData("44h", 5)]
    [InlineData("12/36", 4)]
    [InlineData("seg qua sex", 3)]
    [InlineData(null, 6)]
    public void GetWorkDays_ShouldRecognizeLabelAndCountDays(string? label, int expected)
    {
        var monday = new DateTime(2026, 1, 5);

        Assert.Equal(expected, label.GetWorkDays(monday, monday.AddDays(6)));
    }

    [Fact(DisplayName = "GetWorkDays - Should match GetWorkScheduleDays for every title")]
    public void GetWorkDays_ShouldMatchGetWorkScheduleDays()
    {
        var start = new DateTime(2026, 1, 1);
        var end = new DateTime(2026, 1, 31);
        var table = start.GetWorkScheduleDays(end);

        foreach (var title in GCScriptWorkScheduleExtensions.WorkScheduleTitles)
        {
            Assert.Equal(table[title], title.GetWorkDays(start, end));
        }
    }

    /// <summary>
    /// Períodos sorteados, de um dia a quase três anos, comparados contra o laço de
    /// origem. A contagem é por fórmula fechada, e uma fórmula erra justamente nos
    /// restos - o sorteio cobre combinações de início e duração que a tabela fixa acima
    /// não alcança. A semente é fixa para a falha ser reproduzível.
    /// </summary>
    [Fact(DisplayName = "GetWorkScheduleDays - Random periods should match the literal original version")]
    public void GetWorkScheduleDays_RandomPeriods_ShouldMatchLiteralOriginalVersion()
    {
        var random = new Random(20261005);

        for (var i = 0; i < 500; i++)
        {
            var start = new DateTime(2000, 1, 1).AddDays(random.Next(0, 20_000));
            var end = start.AddDays(random.Next(0, 1_000));
            var result = start.GetWorkScheduleDays(end);

            foreach (var pair in DaysLiteral(start, end))
            {
                Assert.True(
                    pair.Value == result[pair.Key],
                    $"Divergência em {pair.Key} de {start:dd/MM/yyyy} a {end:dd/MM/yyyy}: atual={result[pair.Key]}, origem literal={pair.Value}");
            }
        }
    }

    /// <summary>
    /// O laço de origem avançava a data além do último dia antes de testar o fim, e
    /// estourava com <see cref="ArgumentOutOfRangeException"/> num período que termina
    /// em <see cref="DateTime.MaxValue"/>. 31/12/9999 é sexta-feira.
    /// </summary>
    [Fact(DisplayName = "GetWorkScheduleDays - A period ending at DateTime.MaxValue should not overflow")]
    public void GetWorkScheduleDays_EndingAtMaxValue_ShouldNotOverflow()
    {
        var days = DateTime.MaxValue.AddDays(-6).GetWorkScheduleDays(DateTime.MaxValue);

        Assert.Equal(6, days["6X1"]);
        Assert.Equal(5, days["5X2"]);
        Assert.Equal(4, days["12X36"]);
        Assert.Equal(6, "6x1".GetWorkDays(DateTime.MaxValue.AddDays(-6), DateTime.MaxValue));
    }

    /// <summary>
    /// O calendário inteiro: 3.652.059 dias, começando numa segunda (01/01/0001).
    /// Garante que a contagem cabe em int e não depende do tamanho do período.
    /// </summary>
    [Fact(DisplayName = "GetWorkScheduleDays - The whole calendar should be counted")]
    public void GetWorkScheduleDays_WholeCalendar_ShouldBeCounted()
    {
        var days = DateTime.MinValue.GetWorkScheduleDays(DateTime.MaxValue);

        Assert.Equal(3_130_337, days["6X1"]);
        Assert.Equal(1_826_030, days["12X36"]);
        Assert.Equal(3_043_383, days["5X1"]);
    }

    /// <summary>
    /// Só a data conta: a hora não pode tirar nem pôr dia no período, nem fazer um
    /// período do mesmo dia parecer invertido.
    /// </summary>
    [Fact(DisplayName = "GetWorkScheduleDays - Time of day should be ignored")]
    public void GetWorkScheduleDays_TimeOfDay_ShouldBeIgnored()
    {
        var withTime = new DateTime(2026, 1, 5, 23, 59, 59).GetWorkScheduleDays(new DateTime(2026, 1, 11, 0, 0, 1));
        var withoutTime = new DateTime(2026, 1, 5).GetWorkScheduleDays(new DateTime(2026, 1, 11));

        Assert.Equal(withoutTime, withTime);
        Assert.Equal(1, new DateTime(2026, 1, 5, 18, 0, 0).GetWorkScheduleDays(new DateTime(2026, 1, 5, 8, 0, 0))["6X1"]);
    }

    [Fact(DisplayName = "GetWorkScheduleDays - Should be safe to call concurrently")]
    public void GetWorkScheduleDays_Concurrent_ShouldBeConsistent()
    {
        var start = new DateTime(2026, 1, 1);
        var end = new DateTime(2026, 12, 31);
        var expected = start.GetWorkScheduleDays(end);
        var labels = Labels();
        var sequential = labels.Select(label => label.ToWorkSchedule()).ToList();

        Parallel.For(0, 2_000, i =>
        {
            var index = i % labels.Count;
            var title = labels[index].ToWorkSchedule();

            Assert.Equal(sequential[index], title);
            Assert.Equal(expected[title], labels[index].GetWorkDays(start, end));
        });
    }

    [Theory(DisplayName = "TryToWorkSchedule - Should succeed only when the label is recognized")]
    [InlineData("6x1", true, "6X1")]
    [InlineData("12/36", true, "12X36")]
    [InlineData("TER..QUI", true, "TER..QUI")]
    [InlineData("administrativo", false, null)]
    [InlineData("", false, null)]
    [InlineData("   ", false, null)]
    [InlineData(null, false, null)]
    public void TryToWorkSchedule_ShouldSucceedOnlyWhenRecognized(string? label, bool expectedResult, string? expectedSchedule)
    {
        var result = label.TryToWorkSchedule(out var schedule);

        Assert.Equal(expectedResult, result);
        Assert.Equal(expectedSchedule, schedule);
    }

    /// <summary>
    /// ToWorkSchedule é TryToWorkSchedule com a escala padrão no lugar da falha - e só
    /// isso. Se as duas divergissem em algum rótulo, o diagnóstico apontaria uma coisa
    /// e o cálculo faria outra.
    /// </summary>
    [Fact(DisplayName = "TryToWorkSchedule - Should agree with ToWorkSchedule")]
    public void TryToWorkSchedule_ShouldAgreeWithToWorkSchedule()
    {
        foreach (var label in Labels())
        {
            var expected = label.TryToWorkSchedule(out var schedule) ? schedule : GCScriptWorkScheduleExtensions.DefaultWorkSchedule;

            Assert.Equal(expected, label.ToWorkSchedule());
        }
    }

    [Fact(DisplayName = "GetWorkDays - An inverted period should be rejected")]
    public void GetWorkDays_InvertedPeriod_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => "6x1".GetWorkDays(new DateTime(2026, 2, 1), new DateTime(2026, 1, 1)));
    }
}
