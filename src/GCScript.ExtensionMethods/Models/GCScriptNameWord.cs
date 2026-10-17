namespace GCScript.ExtensionMethods.Models;

/// <summary>
/// [PT-BR] Uma palavra de nome que entrou no emparelhamento, com o lugar onde está no texto comparado e a parceira que recebeu.
/// [EN] A name word that took part in the pairing, with where it sits in the compared text and the partner it got.
/// </summary>
/// <remarks>
/// [PT-BR] Partículas e separadores não viram palavra: o que o texto comparado tem fora das palavras não pesou no percentual.
/// [EN] Particles and separators do not become words: whatever the compared text holds outside the words did not weigh on the percentage.
/// </remarks>
public sealed class GCScriptNameWord
{
    internal GCScriptNameWord(string value, int start, int length, int? partnerIndex, int? score, bool isAbbreviation)
    {
        Value = value;
        Start = start;
        Length = length;
        PartnerIndex = partnerIndex;
        Score = score;
        IsAbbreviation = isAbbreviation;
    }

    /// <summary>
    /// [PT-BR] A palavra como foi comparada, já com as substituições: "jr" aparece aqui como "junior".
    /// [EN] The word as it was compared, replacements applied: "jr" shows here as "junior".
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// [PT-BR] Posição do primeiro caractere da palavra no texto comparado.
    /// [EN] Position of the word's first character in the compared text.
    /// </summary>
    public int Start { get; }

    /// <summary>
    /// [PT-BR] Quantidade de caracteres da palavra no texto comparado - a escrita original, e não a substituída.
    /// [EN] Number of characters of the word in the compared text - the original spelling, not the replaced one.
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// [PT-BR] Índice da parceira na lista de palavras do outro nome, ou nulo se a palavra sobrou sem par.
    /// [EN] Index of the partner in the other name's word list, or null when the word was left without a pair.
    /// </summary>
    public int? PartnerIndex { get; }

    /// <summary>
    /// [PT-BR] Pontuação do par, de 0 a 100, ou nulo se a palavra sobrou sem par.
    /// [EN] Score of the pair, from 0 to 100, or null when the word was left without a pair.
    /// </summary>
    public int? Score { get; }

    /// <summary>
    /// [PT-BR] Indica se o par casou pela inicial: uma das palavras tem uma letra só e ela coincide com a inicial da outra.
    /// [EN] Indicates whether the pair matched by the initial: one of the words is a single letter and it matches the other's initial.
    /// </summary>
    public bool IsAbbreviation { get; }

    /// <summary>
    /// [PT-BR] Indica se a palavra recebeu parceira.
    /// [EN] Indicates whether the word got a partner.
    /// </summary>
    public bool IsPaired => PartnerIndex.HasValue;
}
