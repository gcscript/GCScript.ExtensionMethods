namespace GCScript.ExtensionMethods.Models;

public class StringSimilarityOptions
{
    public bool Levenstein { get; set; } = true;
    public bool JaroWinkler { get; set; } = false;
    public bool Jaccard { get; set; } = false;
    public bool ProcessText { get; set; } = true;
}
