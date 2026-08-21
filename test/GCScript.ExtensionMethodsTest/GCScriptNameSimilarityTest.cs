using GCScript.ExtensionMethods;
using GCScript.ExtensionMethods.Models;

namespace GCScript.ExtensionMethodsTest;

/// <summary>
/// Similaridade entre nomes de pessoa, por emparelhamento de palavras.
/// </summary>
/// <remarks>
/// Os pares vêm de divergências de cadastro que aparecem de verdade. Eles são o
/// contrato do método: a métrica de palavra pode ser trocada, mas nenhuma troca pode
/// derrubar um par da mesma pessoa abaixo de 90 nem promover pessoas diferentes.
/// </remarks>
public class GCScriptNameSimilarityTest
{
    private const int FaixaAlta = 90;

    [Theory(DisplayName = "GetNameSimilarityPercentage - Same person written differently")]
    [InlineData("joao da silva", "joao silva")]
    [InlineData("maria dos santos souza", "maria santos souza")]
    [InlineData("jose c oliveira", "jose carlos oliveira")]
    [InlineData("ana p santos", "ana paula santos")]
    [InlineData("carlos eduardo lima", "lima carlos eduardo")]
    [InlineData("roberto carlos silva junior", "roberto carlos silva jr")]
    [InlineData("luiz fernando alves", "luis fernando alves")]
    [InlineData("patricia goncalves", "patricia gonalves")]
    [InlineData("maria silva", "maria silva de oliveira")]
    [InlineData("anderson rodrigues da silva", "anderson r da silva")]
    [InlineData("marcos antonio lima", "marcos antonio lima")]
    [InlineData("eduardo santos", "eduardo santos santos")]
    [InlineData("maria de fatima silva", "fatima maria da silva")]
    [InlineData("jose pereira filho", "jose pereira")]
    [InlineData("junior aparecido lima", "junior aparecido lima")]
    public void ShouldRecognizeSamePerson(string name, string nameToCompare)
    {
        var result = name.GetNameSimilarityPercentage(nameToCompare);

        Assert.True(result >= FaixaAlta, $"{name} x {nameToCompare} deu {result}%, abaixo de {FaixaAlta}%.");
    }

    [Theory(DisplayName = "GetNameSimilarityPercentage - Different people")]
    [InlineData("jose silva", "jose silveira")]
    [InlineData("carlos silva", "carla silva")]
    [InlineData("ana maria souza", "ana paula souza")]
    [InlineData("joao pedro alves", "pedro henrique alves")]
    [InlineData("junior silva santos", "jose silva santos")]
    [InlineData("junior cesar alves", "marcos cesar alves")]
    [InlineData("neto ferreira costa", "paulo ferreira costa")]
    [InlineData("marcos filho", "marcos silva")]
    [InlineData("silva", "silva santos costa")]
    public void ShouldNotConfuseDifferentPeople(string name, string nameToCompare)
    {
        var result = name.GetNameSimilarityPercentage(nameToCompare);

        Assert.True(result < FaixaAlta, $"{name} x {nameToCompare} deu {result}%, dentro de {FaixaAlta}%.");
    }

    [Theory(DisplayName = "GetNameSimilarityPercentage - Absent name returns zero")]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData("Ana", "")]
    [InlineData("", "Ana")]
    [InlineData("da de dos", "Ana")]
    [InlineData("da de dos", "da de dos")]
    public void ShouldReturnZeroWhenThereIsNoNameToCompare(string? name, string? nameToCompare)
    {
        Assert.Equal(0, name.GetNameSimilarityPercentage(nameToCompare));
    }

    [Fact(DisplayName = "GetNameSimilarityPercentage - Identical names score 100")]
    public void ShouldScoreIdenticalNames()
    {
        Assert.Equal(100, "  JOÃO  DA SILVA  ".GetNameSimilarityPercentage("joao da silva"));
    }

    /// <summary>
    /// A ordem não importa: o emparelhamento procura a melhor parceira em qualquer posição.
    /// </summary>
    [Fact(DisplayName = "GetNameSimilarityPercentage - Word order does not matter")]
    public void ShouldIgnoreWordOrder()
    {
        Assert.Equal(100, "carlos eduardo lima".GetNameSimilarityPercentage("lima carlos eduardo"));
    }

    /// <summary>
    /// Palavra de uma letra vale pela inicial. Como texto, "p" contra "paula" seriam
    /// cinco caracteres de diferença em cinco.
    /// </summary>
    [Theory(DisplayName = "GetNameSimilarityPercentage - Single letter counts as abbreviation")]
    [InlineData("ana p santos", "ana paula santos", true)]
    [InlineData("ana p santos", "ana carla santos", false)]
    public void ShouldTreatSingleLetterAsAbbreviation(string name, string nameToCompare, bool shouldMatch)
    {
        var result = name.GetNameSimilarityPercentage(nameToCompare);

        Assert.Equal(shouldMatch, result >= FaixaAlta);
    }

    /// <summary>
    /// As partículas são opção, não regra fixa: quem comparar nomes de outro idioma
    /// troca a lista.
    /// </summary>
    [Fact(DisplayName = "GetNameSimilarityPercentage - Particles are configurable")]
    public void ShouldAcceptCustomParticles()
    {
        var options = new GCScriptNameSimilarityOptions { Particles = ["del", "la"] };

        Assert.Equal(100, "maria del carmen".GetNameSimilarityPercentage("maria carmen", options));
        Assert.Equal(["maria", "carmen"], "maria del carmen".GetNameWords(options));
    }

    /// <summary>
    /// A abreviação de geração é reescrita em qualquer posição.
    /// </summary>
    /// <remarks>
    /// Restringir à última palavra parece seguro e não é: "Neymar Jr da Silva" carrega
    /// o sufixo no meio, e qualquer sobrenome depois dele empurrava a mesma pessoa para
    /// fora da faixa Alta. A substituição vai só da abreviação para a forma por
    /// extenso, então quem se chama Junior não é tocado.
    /// </remarks>
    [Theory(DisplayName = "GetNameWords - Word replacement applies at any position")]
    [InlineData("roberto silva jr", new[] { "roberto", "silva", "junior" })]
    [InlineData("jr roberto silva", new[] { "junior", "roberto", "silva" })]
    [InlineData("neymar jr da silva", new[] { "neymar", "junior", "silva" })]
    [InlineData("joao da silva", new[] { "joao", "silva" })]
    [InlineData("maria dos santos e souza", new[] { "maria", "santos", "e", "souza" })]
    [InlineData("jose c. oliveira", new[] { "jose", "c", "oliveira" })]
    [InlineData("lima, carlos", new[] { "lima", "carlos" })]
    public void ShouldSplitNameIntoWords(string name, string[] expected)
    {
        Assert.Equal(expected, name.GetNameWords());
    }

    /// <summary>
    /// O sufixo de geração no meio do nome não pode separar a mesma pessoa.
    /// </summary>
    /// <remarks>
    /// É o caso que a regra restrita à última palavra errava: sobrando qualquer
    /// sobrenome depois do "Jr", a comparação caía de 100 para 77 e o cadastro
    /// aparecia como divergente.
    /// </remarks>
    [Theory(DisplayName = "GetNameSimilarityPercentage - Generation suffix in the middle")]
    [InlineData("neymar jr da silva", "neymar junior da silva")]
    [InlineData("jose silva jr da costa", "jose silva junior da costa")]
    [InlineData("neymar jr", "neymar junior")]
    [InlineData("jr roberto silva", "junior roberto silva")]
    public void ShouldMatchGenerationSuffixAnywhere(string name, string nameToCompare)
    {
        Assert.Equal(100, name.GetNameSimilarityPercentage(nameToCompare));
    }

    /// <summary>
    /// Quem se chama Junior continua distinto de quem não se chama: a substituição vai
    /// só num sentido, e não apaga a palavra.
    /// </summary>
    [Fact(DisplayName = "GetNameSimilarityPercentage - Junior as a given name is preserved")]
    public void ShouldPreserveJuniorAsGivenName()
    {
        Assert.True("junior silva santos".GetNameSimilarityPercentage("jose silva santos") < FaixaAlta);
        Assert.True("junior cesar alves".GetNameSimilarityPercentage("marcos cesar alves") < FaixaAlta);
    }

    /// <summary>
    /// A partícula só é descartada quando é a palavra inteira.
    /// </summary>
    /// <remarks>
    /// Comparar por trecho comeria o começo de nomes reais - Dario, Damiana, Douglas,
    /// Dosea, Deolinda - e o nome mutilado ainda casaria com qualquer outro que
    /// sobrasse parecido, sem erro nenhum à vista.
    /// </remarks>
    [Theory(DisplayName = "GetNameWords - Particle is discarded only as a whole word")]
    [InlineData("dario da silva", new[] { "dario", "silva" })]
    [InlineData("damiana dos santos", new[] { "damiana", "santos" })]
    [InlineData("douglas do nascimento", new[] { "douglas", "nascimento" })]
    [InlineData("dosea de oliveira", new[] { "dosea", "oliveira" })]
    [InlineData("deolinda das neves", new[] { "deolinda", "neves" })]
    public void ShouldDiscardParticleOnlyAsWholeWord(string name, string[] expected)
    {
        Assert.Equal(expected, name.GetNameWords());
    }

    /// <summary>
    /// As partículas de origem italiana e francesa também são descartadas.
    /// </summary>
    /// <remarks>
    /// "Di Castro" e "Du Carmo" aparecem no cadastro brasileiro tanto quanto "da Silva",
    /// e sem elas na lista a mesma pessoa escrita com e sem a partícula ficava com uma
    /// palavra sobrando de um lado, pagando o desconto por palavra sem par.
    /// </remarks>
    [Theory(DisplayName = "GetNameWords - Italian, French, Spanish and German particles are discarded too")]
    [InlineData("fulano di castro", new[] { "fulano", "castro" })]
    [InlineData("fulano du carmo", new[] { "fulano", "carmo" })]
    [InlineData("fulano del rey", new[] { "fulano", "rey" })]
    [InlineData("maria della coletta", new[] { "maria", "coletta" })]
    [InlineData("paulo dei santi", new[] { "paulo", "santi" })]
    [InlineData("ana degli esposti", new[] { "ana", "esposti" })]
    [InlineData("marco dello russo", new[] { "marco", "russo" })]
    [InlineData("rosa delle donne", new[] { "rosa", "donne" })]
    [InlineData("carmen la rocca", new[] { "carmen", "rocca" })]
    [InlineData("jose de los santos", new[] { "jose", "santos" })]
    [InlineData("pedro las casas", new[] { "pedro", "casas" })]
    [InlineData("otto von ihering", new[] { "otto", "ihering" })]
    public void ShouldDiscardForeignParticles(string name, string[] expected)
    {
        Assert.Equal(expected, name.GetNameWords());
    }

    /// <summary>
    /// As duas que ficaram de fora de propósito continuam valendo como palavra.
    /// </summary>
    /// <remarks>
    /// "van" é nome do meio vietnamita de verdade (Nguyen Van Minh), e "y" tem uma
    /// letra só - a comparação já a aproveita como inicial de nome do meio. Quem
    /// precisar delas passa a própria lista em <c>Particles</c>.
    /// </remarks>
    [Theory(DisplayName = "GetNameWords - Deliberately excluded particles are kept")]
    [InlineData("nguyen van minh", new[] { "nguyen", "van", "minh" })]
    [InlineData("garcia y lopez", new[] { "garcia", "y", "lopez" })]
    public void ShouldKeepDeliberatelyExcludedParticles(string name, string[] expected)
    {
        Assert.Equal(expected, name.GetNameWords());
    }

    /// <summary>
    /// Nomes que apenas começam com as partículas novas continuam inteiros.
    /// </summary>
    /// <remarks>
    /// É o risco real de ampliar a lista: "Dias", "Diana", "Dinis", "Duarte" e "Dutra"
    /// são nomes de gente, e comê-los deixaria registros diferentes parecendo iguais no
    /// que restasse.
    /// </remarks>
    [Theory(DisplayName = "GetNameWords - Names starting with a particle stay whole")]
    [InlineData("dias silva", new[] { "dias", "silva" })]
    [InlineData("diana costa", new[] { "diana", "costa" })]
    [InlineData("dinis pereira", new[] { "dinis", "pereira" })]
    [InlineData("duarte lima", new[] { "duarte", "lima" })]
    [InlineData("dutra alves", new[] { "dutra", "alves" })]
    [InlineData("dionisio melo", new[] { "dionisio", "melo" })]
    [InlineData("delfino rocha", new[] { "delfino", "rocha" })]
    [InlineData("delmo santos", new[] { "delmo", "santos" })]
    [InlineData("delma ferreira", new[] { "delma", "ferreira" })]
    [InlineData("lara mendes", new[] { "lara", "mendes" })]
    [InlineData("laura antunes", new[] { "laura", "antunes" })]
    [InlineData("deise moreira", new[] { "deise", "moreira" })]
    [InlineData("lazaro pinto", new[] { "lazaro", "pinto" })]
    public void ShouldKeepNamesThatMerelyStartWithTheNewParticles(string name, string[] expected)
    {
        Assert.Equal(expected, name.GetNameWords());
    }

    /// <summary>
    /// Duas pessoas cujo nome apenas começa com partícula continuam distintas: se o
    /// trecho fosse comido, "damiana" e "dario" virariam "miana" e "rio".
    /// </summary>
    [Fact(DisplayName = "GetNameSimilarityPercentage - Names starting with a particle stay intact")]
    public void ShouldNotMutilateNamesThatStartWithAParticle()
    {
        Assert.Equal(100, "damiana dourado".GetNameSimilarityPercentage("damiana dourado"));
        Assert.True("damiana dourado".GetNameSimilarityPercentage("dario dourado") < FaixaAlta);
    }

    /// <summary>
    /// O desconto por palavra sem par é opção: sem ele, um sobrenome a mais de um lado
    /// deixaria de pesar.
    /// </summary>
    [Fact(DisplayName = "GetNameSimilarityPercentage - Unpaired penalty is configurable")]
    public void ShouldApplyUnpairedWordPenalty()
    {
        const string name = "maria silva";
        const string nameToCompare = "maria silva de oliveira";

        var comDesconto = name.GetNameSimilarityPercentage(nameToCompare);
        var semDesconto = name.GetNameSimilarityPercentage(
            nameToCompare,
            new GCScriptNameSimilarityOptions { UnpairedWordPenalty = 0 });

        Assert.Equal(100, semDesconto);
        Assert.True(comDesconto < semDesconto);
    }

    /// <summary>
    /// Opção ausente é escolha de quem chama, não contrato quebrado: sem lista de
    /// partículas não há o que descartar, sem dicionário não há o que substituir.
    /// </summary>
    /// <remarks>
    /// Antes disso, as duas devolviam exceção - e a do dicionário era uma
    /// <c>NullReferenceException</c> pelada, que não diz qual opção faltou.
    /// </remarks>
    [Fact(DisplayName = "GetNameSimilarityPercentage - Absent option lists are tolerated")]
    public void ShouldTolerateAbsentOptionLists()
    {
        var semParticulas = new GCScriptNameSimilarityOptions { Particles = null! };
        var semSubstituicoes = new GCScriptNameSimilarityOptions { WordReplacements = null! };

        Assert.Equal(["ana", "de", "souza"], "ana de souza".GetNameWords(semParticulas));
        Assert.Equal(["roberto", "silva", "jr"], "roberto silva jr".GetNameWords(semSubstituicoes));
        Assert.Equal(100, "ana silva".GetNameSimilarityPercentage("ana silva", semParticulas));
        Assert.Equal(100, "ana silva".GetNameSimilarityPercentage("ana silva", semSubstituicoes));
    }

    /// <summary>
    /// Métrica sem nenhum algoritmo ligado não tem o que medir, e precisa dizer isso.
    /// </summary>
    /// <remarks>
    /// A média de zero elementos estourava lá dentro com "Sequence contains no
    /// elements", que não aponta a opção culpada.
    /// </remarks>
    [Fact(DisplayName = "GetNameSimilarityPercentage - Empty word metric is rejected clearly")]
    public void ShouldRejectWordMetricWithoutAnyAlgorithm()
    {
        var options = new GCScriptNameSimilarityOptions
        {
            WordMetric = new GCScriptStringSimilarityOptions
            {
                Levenstein = false,
                JaroWinkler = false,
                Jaccard = false
            }
        };

        var excecao = Assert.Throws<ArgumentException>(
            () => "ana silva".GetNameSimilarityPercentage("ana silvo", options));

        Assert.Contains("WordMetric", excecao.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// A mesma recusa vale para os dois métodos de similaridade de texto, que sempre
    /// tiveram o problema: sem algoritmo ligado, a média de zero elementos estourava
    /// com "Sequence contains no elements".
    /// </summary>
    [Fact(DisplayName = "GetStringSimilarityPercentage - Empty metric is rejected clearly")]
    public void ShouldRejectStringSimilarityWithoutAnyAlgorithm()
    {
        var options = new GCScriptStringSimilarityOptions
        {
            Levenstein = false,
            JaroWinkler = false,
            Jaccard = false
        };

        var excecao = Assert.Throws<ArgumentException>(
            () => "ana".GetStringSimilarityPercentage("ano", options));

        Assert.Contains("Levenstein", excecao.Message, StringComparison.Ordinal);

        var excecaoLista = Assert.Throws<ArgumentException>(
            () => "ana".GetStringListSimilarityPercentages(["ano", "ana"], options));

        Assert.Contains("Levenstein", excecaoLista.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// A recusa acontece antes de qualquer trabalho, inclusive quando o texto está
    /// vazio - configuração impossível é impossível independente da entrada.
    /// </summary>
    [Fact(DisplayName = "GetStringSimilarityPercentage - Empty metric is rejected before the early exit")]
    public void ShouldRejectEmptyMetricBeforeLookingAtTheText()
    {
        var options = new GCScriptStringSimilarityOptions
        {
            Levenstein = false,
            JaroWinkler = false,
            Jaccard = false
        };

        Assert.Throws<ArgumentException>(() => string.Empty.GetStringSimilarityPercentage(string.Empty, options));
    }

    /// <summary>
    /// As opções são lidas, nunca escritas: uma instância única compartilhada entre
    /// threads precisa produzir sempre o mesmo resultado.
    /// </summary>
    [Fact(DisplayName = "GetNameSimilarityPercentage - Shared options are safe across threads")]
    public void ShouldBeSafeWithSharedOptionsAcrossThreads()
    {
        var compartilhada = new GCScriptNameSimilarityOptions();
        var resultados = new int[8];

        Parallel.For(0, resultados.Length, i =>
        {
            for (var k = 0; k < 2_000; k++)
            {
                resultados[i] = "maria de fatima silva".GetNameSimilarityPercentage("fatima maria da silva", compartilhada);
            }
        });

        Assert.Single(resultados.Distinct());
        Assert.Equal(100, resultados[0]);
    }

    /// <summary>
    /// A métrica de palavra é trocável, e os pares reais continuam classificando igual.
    /// É o que permite trocar de biblioteca sem refazer a calibragem das faixas.
    /// </summary>
    [Fact(DisplayName = "GetNameSimilarityPercentage - Word metric is interchangeable")]
    public void ShouldAcceptAnotherWordMetric()
    {
        var options = new GCScriptNameSimilarityOptions
        {
            WordMetric = new GCScriptStringSimilarityOptions
            {
                Levenstein = true,
                JaroWinkler = true,
                Jaccard = false,
                ProcessText = false
            }
        };

        Assert.True("luiz fernando alves".GetNameSimilarityPercentage("luis fernando alves", options) >= FaixaAlta);
        Assert.True("joao pedro alves".GetNameSimilarityPercentage("pedro henrique alves", options) < FaixaAlta);
    }
}
