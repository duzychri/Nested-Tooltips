namespace NestedTooltips;

/// <summary>
/// Provides the <see cref="TooltipData"/> required for nested tooltips from language-specific files.
/// </summary>
public class TooltipDataProvider : ITooltipDataProvider
{
	private readonly Dictionary<string, string> _languageFilePaths;
	private Dictionary<string, TooltipData> _loadedTooltipData = new();
	private string _currentLanguage;

	/// <summary>
	/// Initializes a new instance of the TooltipDataProvider.
	/// </summary>
	/// <param name="languageFilePaths">A dictionary mapping a language key (e.g., "en") to its file path.</param>
	/// <param name="initialLanguage">The language to load at the beginning.</param>
	public TooltipDataProvider(Dictionary<string, string> languageFilePaths, string preferredLanguage, string fallbackLanguage)
	{
		_languageFilePaths = languageFilePaths ?? throw new ArgumentNullException(nameof(languageFilePaths));
		
		// Versuche, die bevorzugte Sprache zu laden
		if (languageFilePaths.TryGetValue(preferredLanguage, out var filePath))
		{
			LoadLanguage(filePath);
			_currentLanguage = preferredLanguage;
		}
		// Ansonsten versuche, die Fallback-Sprache zu laden
		else if (languageFilePaths.TryGetValue(fallbackLanguage, out var fallbackPath))
		{
			GD.Print($"Bevorzugte Sprache '{preferredLanguage}' nicht gefunden. Lade Fallback-Sprache '{fallbackLanguage}'.");
			LoadLanguage(fallbackPath);
			_currentLanguage = fallbackLanguage;
		}
		else
		{
			GD.PrintErr("Keine gültige Sprachdatei (weder bevorzugt noch Fallback) gefunden.");
		}
	}

	/// <summary>
	/// Gets or sets the current language. Changing the language will reload the tooltip data.
	/// </summary>
	public string CurrentLanguage
	{
		get => _currentLanguage;
		set
		{
			if (_currentLanguage != value)
			{
				LoadLanguage(value);
				_currentLanguage = value;
			}
		}
	}

	/// <summary>
	/// Loads the tooltip data for a specific language from its JSON file.
	/// </summary>
	/// <param name="filePath">The path of the language to load.</param>
	private void LoadLanguage(string filePath)
	{
		if (!Godot.FileAccess.FileExists(filePath))
			{
				GD.PrintErr($"Tooltip-Datei nicht gefunden: {filePath}");
				return;
			}

			try
			{
				using var file = Godot.FileAccess.Open(filePath, Godot.FileAccess.ModeFlags.Read);
				string content = file.GetAsText();
				var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
				var data = JsonSerializer.Deserialize<Dictionary<string, TooltipData>>(content, options);
				
				_loadedTooltipData = data ?? new Dictionary<string, TooltipData>();
				GD.Print($"Successfully loaded tooltips from: {filePath}");
			}
			catch (Exception ex)
			{
				GD.PrintErr($"Error loading or parsing language file for '{filePath}': {ex.Message}");
				_loadedTooltipData = new Dictionary<string, TooltipData>();
			}
	}

	/// <inheritdoc />
	public TooltipData? GetTooltipData(string id)
	{
		if (_loadedTooltipData.TryGetValue(id, out TooltipData? data))
		{
			return data;
		}

		GD.PushWarning($"TooltipData with ID '{id}' not found for the current language ('{_currentLanguage}').");
		return null;
	}
}
