namespace NestedTooltips.DemoScene;

public partial class DemoSceneManager : Node
{
    [ExportGroup("Tooltip Language Configuration")]
    [Export] private LanguageMapping[] _languageMappings = [];
    [Export] private string _fallbackLanguageCode = "en";

    public override void _Ready()
    {
        var languagePaths = new Dictionary<string, string>();
        foreach (var mapping in _languageMappings)
        {
            if (mapping != null && !string.IsNullOrEmpty(mapping.LanguageCode))
            {
                languagePaths[mapping.LanguageCode] = mapping.FilePath;
            }
        }
        var sceneSpecificProvider = new BasicTooltipDataProvider(
            languagePaths,
            TranslationServer.GetLocale(),
            _fallbackLanguageCode
        );
        TooltipService.TooltipDataProvider = sceneSpecificProvider;
    }
}
