using GCScript.ExtensionMethods;
using GCScript.ExtensionMethods.Models;

namespace GCScript.ExtensionMethodsTest;

/// <summary>
/// O emparelhamento exposto por GetNameSimilarity: posições, parceiras e o percentual.
/// </summary>
public class GCScriptNameSimilarityResultTest
{
    private static readonly GCScriptNameSimilarityOptions SemProcessText = new() { ProcessText = false };

    [Fact(DisplayName = "GetNameSimilarity - Words carry their position and partner")]
    public void ShouldExposePositionAndPartner()
    {
        var resultado = "joao da silva".GetNameSimilarity("joao silva", SemProcessText);

        Assert.Equal(100, resultado.Percentage);
        Assert.Collection(
            resultado.Words,
            joao => AssertPalavra(joao, "joao", start: 0, length: 4, partner: 0, score: 100),
            silva => AssertPalavra(silva, "silva", start: 8, length: 5, partner: 1, score: 100));
    }

    /// <summary>
    /// A partícula não vira palavra: fica fora das posições, como fica fora do percentual.
    /// </summary>
    [Fact(DisplayName = "GetNameSimilarity - Particles are not words")]
    public void ShouldLeaveParticlesOutOfTheWords()
    {
        var resultado = "joao da silva".GetNameSimilarity("joao silva", SemProcessText);

        Assert.DoesNotContain(resultado.Words, palavra => palavra.Value == "da");
    }

    [Fact(DisplayName = "GetNameSimilarity - Single letter pair is flagged as abbreviation")]
    public void ShouldFlagAbbreviation()
    {
        var resultado = "ana p santos".GetNameSimilarity("ana paula santos", SemProcessText);

        var inicial = resultado.Words[1];
        Assert.True(inicial.IsAbbreviation);
        Assert.Equal(95, inicial.Score);
        Assert.Equal(1, inicial.PartnerIndex);
        Assert.True(resultado.WordsToCompare[1].IsAbbreviation);
        Assert.False(resultado.Words[0].IsAbbreviation);
    }

    [Fact(DisplayName = "GetNameSimilarity - Surplus word has no partner")]
    public void ShouldLeaveSurplusWordWithoutPartner()
    {
        var resultado = "maria silva".GetNameSimilarity("maria silva de oliveira", SemProcessText);

        Assert.All(resultado.Words, palavra => Assert.True(palavra.IsPaired));
        var oliveira = resultado.WordsToCompare[2];
        Assert.Equal("oliveira", oliveira.Value);
        Assert.False(oliveira.IsPaired);
        Assert.Null(oliveira.Score);
        Assert.Equal(90, resultado.Percentage);
    }

    /// <summary>
    /// A palavra substituída é comparada pela forma por extenso, mas a posição aponta a
    /// escrita original - é ela que está no texto para ser destacada.
    /// </summary>
    [Fact(DisplayName = "GetNameSimilarity - Replaced word points at its original spelling")]
    public void ShouldPointReplacedWordAtTheOriginalSpelling()
    {
        var resultado = "roberto silva jr".GetNameSimilarity("roberto silva junior", SemProcessText);

        var sufixo = resultado.Words[2];
        Assert.Equal("junior", sufixo.Value);
        Assert.Equal("jr", resultado.Text.Substring(sufixo.Start, sufixo.Length));
        Assert.Equal(100, sufixo.Score);
    }

    /// <summary>
    /// Com ProcessText ligado, as posições valem para o texto tratado, que o resultado expõe.
    /// </summary>
    [Fact(DisplayName = "GetNameSimilarity - Positions refer to the processed text")]
    public void ShouldReferPositionsToTheProcessedText()
    {
        var resultado = "  JOÃO  DA SILVA  ".GetNameSimilarity("joao da silva");

        Assert.Equal("  JOÃO  DA SILVA  ".ProcessText(), resultado.Text);
        Assert.Equal(["joao", "silva"], resultado.Words.Select(palavra => resultado.Text.Substring(palavra.Start, palavra.Length)));
    }

    [Theory(DisplayName = "GetNameSimilarity - Empty name leaves the other without partners")]
    [InlineData("ana silva", "")]
    [InlineData("ana silva", null)]
    [InlineData("ana silva", "da de")]
    public void ShouldLeaveWordsWithoutPartnersWhenTheOtherNameIsEmpty(string name, string? nameToCompare)
    {
        var resultado = name.GetNameSimilarity(nameToCompare);

        Assert.Equal(0, resultado.Percentage);
        Assert.Equal(2, resultado.Words.Count);
        Assert.All(resultado.Words, palavra => Assert.False(palavra.IsPaired));
        Assert.Empty(resultado.WordsToCompare);
    }

    [Fact(DisplayName = "GetNameSimilarity - Empty word metric is rejected clearly")]
    public void ShouldRejectWordMetricWithoutAnyAlgorithm()
    {
        var options = new GCScriptNameSimilarityOptions
        {
            WordMetric = new GCScriptStringSimilarityOptions { Levenstein = false, JaroWinkler = false, Jaccard = false }
        };

        Assert.Throws<ArgumentException>(() => "ana".GetNameSimilarity("ana", options));
    }

    /// <summary>
    /// O percentual e as palavras saem iguais aos do algoritmo anterior à exposição do
    /// emparelhamento, em milhares de pares gerados - com partícula, abreviação, sufixo,
    /// acento, caixa, pontuação e espaço sobrando - e em cada combinação de opções.
    /// </summary>
    /// <remarks>
    /// A versão anterior está copiada literalmente em <see cref="AlgoritmoAnterior"/>: é a
    /// referência, e por isso não pode chamar nada que esta mudança tocou.
    /// </remarks>
    [Theory(DisplayName = "GetNameSimilarity - Same percentage and words as the previous algorithm")]
    [MemberData(nameof(CombinacoesDeOpcoes))]
    public void ShouldMatchThePreviousAlgorithm(string descricao, GCScriptNameSimilarityOptions options)
    {
        var aleatorio = new Random(20261017);

        for (var i = 0; i < 1_500; i++)
        {
            var nome = GerarNome(aleatorio);
            var outro = GerarNome(aleatorio);

            var resultado = nome.GetNameSimilarity(outro, options);

            Assert.True(
                AlgoritmoAnterior.Percentual(nome, outro, options) == resultado.Percentage,
                $"[{descricao}] \"{nome}\" x \"{outro}\": esperado {AlgoritmoAnterior.Percentual(nome, outro, options)}, veio {resultado.Percentage}.");
            Assert.Equal(AlgoritmoAnterior.Palavras(nome, options), nome.GetNameWords(options));
            AssertEmparelhamentoCoerente(resultado, options);
        }
    }

    public static TheoryData<string, GCScriptNameSimilarityOptions> CombinacoesDeOpcoes() => new()
    {
        { "padrão", new GCScriptNameSimilarityOptions() },
        { "sem ProcessText", new GCScriptNameSimilarityOptions { ProcessText = false } },
        { "sem desconto", new GCScriptNameSimilarityOptions { UnpairedWordPenalty = 0 } },
        { "abreviação 80", new GCScriptNameSimilarityOptions { AbbreviationScore = 80 } },
        { "sem separadores", new GCScriptNameSimilarityOptions { Separators = null! } },
        { "sem partículas", new GCScriptNameSimilarityOptions { Particles = null! } },
        { "sem substituições", new GCScriptNameSimilarityOptions { WordReplacements = null! } },
        {
            "outra métrica",
            new GCScriptNameSimilarityOptions
            {
                WordMetric = new GCScriptStringSimilarityOptions { Levenstein = true, JaroWinkler = true, Jaccard = true, ProcessText = false }
            }
        },
    };

    private static readonly string[] Vocabulario =
    [
        "joao", "JOÃO", "jose", "José", "maria", "Maria", "ana", "ANA", "paula", "silva", "silvo", "souza", "sousa",
        "santos", "oliveira", "lima", "carla", "carlos", "p", "c", "a", "jr", "junior", "filho", "neto",
        "da", "de", "dos", "DA", "del", "von", "van", "y", "e", "d'avila", "o'neil", "silva-santos", "dario", "dias",
    ];

    private static readonly string[] Ligacoes = [" ", " ", " ", "  ", ", ", ". ", " - ", "\t", " \t ", "\t, "];

    private static string GerarNome(Random aleatorio)
    {
        var quantidade = aleatorio.Next(0, 6);
        var nome = string.Empty;
        for (var i = 0; i < quantidade; i++)
        {
            nome += (i == 0 ? string.Empty : Ligacoes[aleatorio.Next(Ligacoes.Length)]) + Vocabulario[aleatorio.Next(Vocabulario.Length)];
        }
        return aleatorio.Next(10) == 0 ? $"  {nome} " : nome;
    }

    /// <summary>
    /// Parceira é recíproca, com a mesma pontuação dos dois lados, e cada posição aponta
    /// para a escrita da palavra no texto comparado.
    /// </summary>
    private static void AssertEmparelhamentoCoerente(GCScriptNameSimilarityResult resultado, GCScriptNameSimilarityOptions options)
    {
        AssertLado(resultado.Words, resultado.Text, resultado.WordsToCompare, options);
        AssertLado(resultado.WordsToCompare, resultado.TextToCompare, resultado.Words, options);
        Assert.Equal(
            Math.Min(resultado.Words.Count, resultado.WordsToCompare.Count),
            resultado.Words.Count(palavra => palavra.IsPaired));
    }

    private static void AssertLado(
        IReadOnlyList<GCScriptNameWord> palavras,
        string texto,
        IReadOnlyList<GCScriptNameWord> outras,
        GCScriptNameSimilarityOptions options)
    {
        for (var indice = 0; indice < palavras.Count; indice++)
        {
            var palavra = palavras[indice];
            var escrita = texto.Substring(palavra.Start, palavra.Length);
            var esperado = options.WordReplacements is { } substituicoes && substituicoes.TryGetValue(escrita, out var porExtenso) ? porExtenso : escrita;
            Assert.Equal(esperado, palavra.Value);

            if (palavra.PartnerIndex is int parceira)
            {
                Assert.Equal(indice, outras[parceira].PartnerIndex);
                Assert.Equal(palavra.Score, outras[parceira].Score);
                Assert.Equal(palavra.IsAbbreviation, outras[parceira].IsAbbreviation);
            }
        }
    }

    private static void AssertPalavra(GCScriptNameWord palavra, string value, int start, int length, int partner, int score)
    {
        Assert.Equal(value, palavra.Value);
        Assert.Equal(start, palavra.Start);
        Assert.Equal(length, palavra.Length);
        Assert.Equal(partner, palavra.PartnerIndex);
        Assert.Equal(score, palavra.Score);
    }

    /// <summary>
    /// Cópia literal de GetNameSimilarityPercentage e GetNameWords como eram antes de o
    /// emparelhamento ser exposto (versão 1.5.0).
    /// </summary>
    private static class AlgoritmoAnterior
    {
        public static int Percentual(string? text, string? textToCompare, GCScriptNameSimilarityOptions options)
        {
            if (options.ProcessText) { text = text.ProcessText(); textToCompare = textToCompare.ProcessText(); }
            if (text.IsNullOrWhiteSpace() || textToCompare.IsNullOrWhiteSpace()) { return 0; }

            List<string> words = Palavras(text, options);
            List<string> wordsToCompare = Palavras(textToCompare, options);
            if (words.Count == 0 || wordsToCompare.Count == 0) { return 0; }

            List<string> shorter = words.Count <= wordsToCompare.Count ? words : wordsToCompare;
            List<string> available = new(words.Count <= wordsToCompare.Count ? wordsToCompare : words);
            List<int> scores = new(shorter.Count);

            foreach (string word in shorter)
            {
                int best = int.MinValue;
                int chosen = 0;
                for (int i = 0; i < available.Count; i++)
                {
                    int score = Pontuar(word, available[i], options);
                    if (score > best) { best = score; chosen = i; }
                }
                scores.Add(best);
                available.RemoveAt(chosen);
            }

            double average = scores.Average() - (available.Count * options.UnpairedWordPenalty);
            return Math.Max(0, Math.Min(100, (int)Math.Floor(average)));
        }

        public static List<string> Palavras(string? text, GCScriptNameSimilarityOptions options)
        {
            if (options.ProcessText) { text = text.ProcessText(); }
            if (text.IsNullOrWhiteSpace()) { return new(); }

            List<string> particles = options.Particles ?? [];
            List<string> words = text!.Split(options.Separators, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(x => x.Trim())
                                     .Where(x => x.Length > 0 && !particles.Contains(x, StringComparer.Ordinal))
                                     .ToList();

            if (options.WordReplacements is { Count: > 0 })
            {
                for (int i = 0; i < words.Count; i++)
                {
                    if (options.WordReplacements.TryGetValue(words[i], out string? replacement)) { words[i] = replacement; }
                }
            }
            return words;
        }

        private static int Pontuar(string word, string wordToCompare, GCScriptNameSimilarityOptions options)
        {
            if (word.Length == 1 || wordToCompare.Length == 1) { return word[0] == wordToCompare[0] ? options.AbbreviationScore : 0; }
            return (int)Math.Round(word.GetStringSimilarityPercentage(wordToCompare, options.WordMetric));
        }
    }
}
