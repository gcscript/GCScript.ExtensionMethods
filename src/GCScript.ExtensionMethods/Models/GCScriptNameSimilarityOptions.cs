namespace GCScript.ExtensionMethods.Models;

/// <summary>
/// [PT-BR] Opções do cálculo de similaridade entre nomes de pessoa.
/// [EN] Options for the person name similarity calculation.
/// </summary>
public class GCScriptNameSimilarityOptions
{
    /// <summary>
    /// [PT-BR] Palavras descartadas por não identificarem ninguém. O padrão é pt-BR; troque a lista para outro idioma.
    /// [EN] Words discarded because they identify nobody. The default is pt-BR; replace the list for another language.
    /// </summary>
    /// <remarks>
    /// [PT-BR] O critério de entrada é único: a palavra NUNCA existe sozinha como nome de pessoa. Descartar palavra que possa ser nome é o erro perigoso, porque deixa registros diferentes parecendo iguais no que resta. Por isso "junior", "filho" e "neto" ficam de fora - são sufixo de geração E nome próprio, e removê-los faria "Marcos Filho" e "Marcos Silva" ficarem idênticos. "e" e "y" também ficam de fora: têm uma letra só, e a comparação já as aproveita como inicial de nome do meio. "van" fica de fora de propósito, apesar de Van Dijk: é nome do meio vietnamita de verdade (Nguyen Van Minh). O descarte é por palavra inteira, então "Dias", "Duarte", "Diana", "Delfino" e "Lara" não são tocados.
    /// [EN] There is a single admission rule: the word NEVER stands alone as a person name. Discarding a word that could be a name is the dangerous mistake, because it leaves different records looking alike in what remains. That is why "junior", "filho" and "neto" stay out - they are both a generation suffix AND a given name, and dropping them would make "Marcos Filho" and "Marcos Silva" identical. "e" and "y" stay out too: they are single letters, and the comparison already uses them as a middle name initial. "van" is deliberately out despite Van Dijk: it is a real Vietnamese middle name (Nguyen Van Minh). Discarding matches whole words, so "Dias", "Duarte", "Diana", "Delfino" and "Lara" are untouched.
    /// </remarks>
    public List<string> Particles { get; set; } =
    [
        "da", "das", "de", "do", "dos",                                 // pt-BR
        "di", "du", "del", "della", "delle", "dello", "degli", "dei",   // italiano e frances
        "la", "las", "los",                                             // espanhol
        "von"                                                           // alemao
    ];

    /// <summary>
    /// [PT-BR] Caracteres que separam as palavras do nome.
    /// [EN] Characters that separate the name words.
    /// </summary>
    public char[] Separators { get; set; } = [' ', ',', '.', '-', '\''];

    /// <summary>
    /// [PT-BR] Substituições aplicadas a qualquer palavra do nome, para a abreviação e a forma por extenso casarem.
    /// [EN] Replacements applied to any word of the name, so the abbreviation and the spelled out form match.
    /// </summary>
    /// <remarks>
    /// [PT-BR] Vale em qualquer posição, e não só na última: "Neymar Jr da Silva" tem o sufixo de geração no meio, e restringir ao fim deixava a mesma pessoa 23 pontos abaixo de "Neymar Junior da Silva". A substituição é sempre da abreviação para a forma por extenso - "jr" não é nome de ninguém, mas "junior" é, e substituir numa direção só mantém intacto quem se chama Junior.
    /// [EN] It applies at any position, not only the last: "Neymar Jr da Silva" carries the generation suffix in the middle, and restricting it to the end left the same person 23 points below "Neymar Junior da Silva". The replacement always goes from the abbreviation to the spelled out form - "jr" is nobody's given name, but "junior" is, and replacing one way only keeps anyone named Junior intact.
    /// </remarks>
    public Dictionary<string, string> WordReplacements { get; set; } = new() { ["jr"] = "junior" };

    /// <summary>
    /// [PT-BR] Pontuação de uma palavra de uma letra cuja inicial coincide. "p" contra "paula" é abreviação correta, mas como texto são cinco caracteres de diferença em cinco.
    /// [EN] Score for a single letter word whose initial matches. "p" against "paula" is a correct abbreviation, yet as text it is five characters apart out of five.
    /// </summary>
    public int AbbreviationScore { get; set; } = 95;

    /// <summary>
    /// [PT-BR] Desconto por palavra que sobrou sem par, aplicado ao resultado final.
    /// [EN] Penalty per word left without a pair, applied to the final result.
    /// </summary>
    public int UnpairedWordPenalty { get; set; } = 10;

    /// <summary>
    /// [PT-BR] Métrica usada para comparar DUAS PALAVRAS já emparelhadas. ProcessText fica desligado porque o nome inteiro já foi tratado na entrada.
    /// [EN] Metric used to compare TWO PAIRED WORDS. ProcessText is off because the whole name was already processed on entry.
    /// </summary>
    public GCScriptStringSimilarityOptions WordMetric { get; set; } = new() { Levenstein = true, JaroWinkler = false, Jaccard = false, ProcessText = false };

    /// <summary>
    /// [PT-BR] Trata os dois nomes com ProcessText antes de comparar.
    /// [EN] Processes both names with ProcessText before comparing.
    /// </summary>
    public bool ProcessText { get; set; } = true;
}
