namespace NestedTooltips.DemoScene;

public partial class DemoSceneManager : Node
{
    [ExportGroup("Settings Controls")]
    [Export] private OptionButton _lockTypeSelector = null!;
    [Export] private NumberedSlider _showDelaySlider = null!;
    [Export] private NumberedSlider _lockDelaySlider = null!;
    [Export] private NumberedSlider _unlockDelaySlider = null!;

    [ExportGroup("Tooltip Language Configuration")]
    [Export] private LanguageMapping[] _languageMappings = [];
    [Export] private string _fallbackLanguageCode = "en";
    [Export] private OptionButton _languageSelector = null!;

    public override void _Ready()
    {
        // Configure the tooltip service.
        Dictionary<string, string> languagePaths = [];
        foreach (LanguageMapping mapping in _languageMappings)
        {
            if (mapping != null && !string.IsNullOrEmpty(mapping.LanguageCode))
            {
                languagePaths[mapping.LanguageCode] = mapping.FilePath;
            }
        }
        BasicTooltipDataProvider sceneSpecificProvider = new(
            languagePaths,
            "en",
            _fallbackLanguageCode
        );
        TooltipService.TooltipDataProvider = sceneSpecificProvider;

        // Set up the language selector.
        for (int n = 0; n < _languageMappings.Length; n++)
        {
            LanguageMapping mapping = _languageMappings[n];
            if (mapping != null && !string.IsNullOrEmpty(mapping.LanguageCode))
            {
                _languageSelector.AddItem(mapping.LanguageCode, n);
                _languageSelector.ItemSelected += (long index) =>
                {
                    if (index < 0 || index >= _languageMappings.Length)
                    {
                        GD.PushError("Invalid language selected.");
                        return;
                    }

                    GD.PushWarning($"TODO: Language set to: {mapping.LanguageCode}");
                };
            }
        }

        // Set up the settings controls.
        _lockTypeSelector.AddItem("Timer Lock", (int)TooltipLockMode.TimerLock);
        _lockTypeSelector.AddItem("Action Lock", (int)TooltipLockMode.ActionLock);
        _lockTypeSelector.ItemSelected += OnLockTypeSelected;

        _showDelaySlider.Value = TooltipService.Settings.ShowDelay;
        _showDelaySlider.ValueChanged += (double value) =>
        {
            TooltipService.Settings = TooltipService.Settings with { ShowDelay = (float)value };
        };

        _lockDelaySlider.Value = TooltipService.Settings.LockDelay;
        _lockDelaySlider.ValueChanged += (double value) =>
        {
            TooltipService.Settings = TooltipService.Settings with { LockDelay = (float)value };
        };

        _unlockDelaySlider.Value = TooltipService.Settings.UnlockDelay;
        _unlockDelaySlider.ValueChanged += (double value) =>
        {
            TooltipService.Settings = TooltipService.Settings with { UnlockDelay = (float)value };
        };
    }

    private void OnLockTypeSelected(long index)
    {
        if (index < 0 || index >= _lockTypeSelector.GetItemCount())
        {
            GD.PushWarning("Invalid lock type selected.");
            return;
        }

        TooltipLockMode selectedMode = (TooltipLockMode)index;
        TooltipService.Settings = TooltipService.Settings with { LockMode = selectedMode };
    }
}
