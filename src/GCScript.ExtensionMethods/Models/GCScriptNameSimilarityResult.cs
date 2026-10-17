namespace GCScript.ExtensionMethods.Models;

/// <summary>
/// [PT-BR] Resultado da comparação de dois nomes de pessoa: o percentual e o emparelhamento que o produziu.
/// [EN] Result of comparing two person names: the percentage and the pairing that produced it.
/// </summary>
/// <remarks>
/// [PT-BR] O emparelhamento vem junto para quem precisa explicar o percentual - destacar, numa tela, as palavras que sobraram ou divergiram. As posições das palavras se referem a <see cref="Text"/> e <see cref="TextToCompare"/>, que são os nomes como foram comparados: depois do ProcessText quando a opção está ligada, ou os próprios textos de entrada quando está desligada.
/// [EN] The pairing comes along for whoever needs to explain the percentage - highlighting, on a screen, the words left over or diverging. Word positions refer to <see cref="Text"/> and <see cref="TextToCompare"/>, which are the names as compared: after ProcessText when the option is on, or the input texts themselves when it is off.
/// </remarks>
public sealed class GCScriptNameSimilarityResult
{
    internal GCScriptNameSimilarityResult(
        int percentage,
        string text,
        string textToCompare,
        IReadOnlyList<GCScriptNameWord> words,
        IReadOnlyList<GCScriptNameWord> wordsToCompare)
    {
        Percentage = percentage;
        Text = text;
        TextToCompare = textToCompare;
        Words = words;
        WordsToCompare = wordsToCompare;
    }

    /// <summary>
    /// [PT-BR] A porcentagem de similaridade, de 0 a 100 - a mesma de GetNameSimilarityPercentage.
    /// [EN] The similarity percentage, from 0 to 100 - the same as GetNameSimilarityPercentage.
    /// </summary>
    public int Percentage { get; }

    /// <summary>
    /// [PT-BR] O primeiro nome como foi comparado.
    /// [EN] The first name as compared.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// [PT-BR] O segundo nome como foi comparado.
    /// [EN] The second name as compared.
    /// </summary>
    public string TextToCompare { get; }

    /// <summary>
    /// [PT-BR] As palavras do primeiro nome, na ordem em que aparecem; PartnerIndex aponta para <see cref="WordsToCompare"/>.
    /// [EN] The first name's words, in the order they appear; PartnerIndex points into <see cref="WordsToCompare"/>.
    /// </summary>
    public IReadOnlyList<GCScriptNameWord> Words { get; }

    /// <summary>
    /// [PT-BR] As palavras do segundo nome, na ordem em que aparecem; PartnerIndex aponta para <see cref="Words"/>.
    /// [EN] The second name's words, in the order they appear; PartnerIndex points into <see cref="Words"/>.
    /// </summary>
    public IReadOnlyList<GCScriptNameWord> WordsToCompare { get; }
}
