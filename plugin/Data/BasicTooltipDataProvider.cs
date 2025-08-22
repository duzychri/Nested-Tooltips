#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace NestedTooltips;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Provides the <see cref="TooltipData"/> required for nested tooltips from language-specific files.
/// </summary>
public class BasicTooltipDataProvider : ITooltipDataProvider
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly Dictionary<string, TooltipData> _loadedTooltipData = [];

    public static BasicTooltipDataProvider Empty { get; } = new();

    private BasicTooltipDataProvider()
    { }

    /// <summary>
    /// Initializes a new instance of the TooltipDataProvider.
    /// </summary>
    /// <param name="filePath">The path to the JSON file containing tooltip data.</param>
    public BasicTooltipDataProvider(string filePath)
    {
        if (!Godot.FileAccess.FileExists(filePath))
        {
            GD.PrintErr($"Tooltip data file was not found at path: '{filePath}'");
            return;
        }

        try
        {
            using Godot.FileAccess file = Godot.FileAccess.Open(filePath, Godot.FileAccess.ModeFlags.Read);
            string content = file.GetAsText();
            Dictionary<string, TooltipData>? data = JsonSerializer.Deserialize<Dictionary<string, TooltipData>>(content, SerializerOptions);

            _loadedTooltipData = data ?? [];
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Parsing tooltip data file at path: '{filePath}'\n{ex.Message}");
        }
    }

    /// <inheritdoc />
    public TooltipData? GetTooltipData(string id)
    {
        if (_loadedTooltipData.TryGetValue(id, out TooltipData? data))
        {
            return data;
        }

        return null;
    }
}
