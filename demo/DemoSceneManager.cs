namespace NestedTooltips.DemoScene;

public partial class DemoSceneManager : Node
{
	[ExportGroup("Tooltip Language Configuration")]
	[Export]
	private LanguageMapping[] _languageMappings = System.Array.Empty<LanguageMapping>();

	[Export]
	private string _fallbackLanguageCode = "en";

	[ExportGroup("Scene-Specific Nodes")]
	[Export] private string _text = "Hello World!";
	[Export] private RichTextLabel _demoTextLabel = null!;

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
		var sceneSpecificProvider = new TooltipDataProvider(
			languagePaths,
			TranslationServer.GetLocale(),
			_fallbackLanguageCode
		);
		TooltipService.TooltipDataProvider = sceneSpecificProvider;
		GD.Print(_text);
	}

	private ITooltip? _tooltipComponent;

}
