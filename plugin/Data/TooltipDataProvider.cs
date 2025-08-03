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
	public TooltipDataProvider(Dictionary<string, string> languageFilePaths, string initialLanguage)
	{
		_languageFilePaths = languageFilePaths ?? throw new ArgumentNullException(nameof(languageFilePaths));
		_currentLanguage = initialLanguage;
		
		// Lade die initial eingestellte Sprache
		LoadLanguage(_currentLanguage);
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
	/// <param name="languageKey">The key of the language to load (e.g., "en").</param>
	private void LoadLanguage(string languageKey)
	{
		if (!_languageFilePaths.TryGetValue(languageKey, out string? path))
		{
			GD.PrintErr($"Language '{languageKey}' not found in the provided paths.");
			return;
		}

		try
		{
			using var file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read);
			if (file == null)
			{
				GD.PrintErr($"Failed to open file for language '{languageKey}' at path: {path}");
				_loadedTooltipData = new Dictionary<string, TooltipData>(); // Leeren, um Fehler zu vermeiden
				return;
			}
			
			string content = file.GetAsText();
			var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
			var data = JsonSerializer.Deserialize<Dictionary<string, TooltipData>>(content, options);
			
			_loadedTooltipData = data ?? new Dictionary<string, TooltipData>();
			GD.Print($"Successfully loaded language: {languageKey}");
		}
		catch (Exception ex)
		{
			GD.PrintErr($"Error loading or parsing language file for '{languageKey}': {ex.Message}");
			_loadedTooltipData = new Dictionary<string, TooltipData>(); // Sicherstellen, dass keine alten Daten verwendet werden
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
