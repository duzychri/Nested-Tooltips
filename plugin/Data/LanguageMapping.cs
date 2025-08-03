namespace NestedTooltips
{
    [GlobalClass]
    public partial class LanguageMapping : Resource
    {
        [Export] public string LanguageCode { get; set; } = "";
        [Export(PropertyHint.File, "*.json")] public string FilePath { get; set; } = "";
    }
}