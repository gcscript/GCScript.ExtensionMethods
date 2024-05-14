namespace GCScript.ExtensionMethods.Models;

public class GCScriptStringSimilarityOptions
{
    public bool Levenstein { get; set; } = true;
    public bool JaroWinkler { get; set; } = false;
    public bool Jaccard { get; set; } = false;
    public bool ProcessText { get; set; } = true;
}
