namespace NestedTooltips;

public partial class TooltipService : GodotSingelton<TooltipService>
{
    private const string defaultTooltipPrefabPath = "res://demo/DemoTooltip.tscn";

    [Export] private Control _tooltipsParent = null!;

    private static readonly TooltipDataProvider _dataProvider = new();
    private static readonly Dictionary<ITooltipComponent, TooltipComponent> _activeTooltips = [];

    #region API

    /// <summary>
    /// The path to the tooltip prefab that is used to create new tooltips.
    /// Set this if you want to use a different tooltip prefab than the default one.
    /// </summary>
    public static string? TooltipPrefabPath
    {
        get
        {
            GD.Print("TODO: TooltipService: TooltipPrefabPath getter called");
            return null;
        }
        set
        {
            GD.Print($"TODO: TooltipService: TooltipPrefabPath setter called with {value}");
        }
    }

    /// <summary>
    /// Determines the behaviour of the tooltip system.
    /// </summary>
    /// <remarks>
    /// <b>Note:</b> When the settings are changed, all currently active tooltips will be closed!
    /// </remarks>
    public static TooltipSettings Settings
    {
        get
        {
            GD.Print("TODO: TooltipService: Settings getter called");
            return TooltipSettings.Default;
        }
        set
        {
            GD.Print($"TODO: TooltipService: Settings setter called with {value}");
        }
    }

    /// <summary>
    /// All currently active tooltips, including all nested ones.
    /// </summary>
    public static IEnumerable<ITooltipComponent> ActiveTooltips => _activeTooltips.Keys;

    /// <summary>
    /// Creates a new tooltip at the given position with the given pivot and text.
    /// </summary>
    /// <param name="position">The position of the tooltip.</param>
    /// <param name="pivot">The pivot of the tooltip. Determines where the tooltip is anchored in relation to the position.</param>
    /// <param name="text">The bbcode formatted text of the tooltip.</param>
    /// <returns>The created tooltip component.</returns>
    public static ITooltipComponent ShowTooltip(Vector2 position, TooltipPivot pivot, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        // Create the tooltip and set its text.
        TooltipComponent tooltip = CreateTooltip();
        tooltip.Text = text;

        // Calculate the position of the tooltip based on the pivot and the size of the tooltip.
        Vector2 placementPosition = CalculateNewTooltipLocation(position, pivot, tooltip.Size);
        placementPosition = CalculatePositionFromPivot(placementPosition, pivot, tooltip.Size);
        tooltip.Position = placementPosition;

        return tooltip;
    }

    /// <summary>
    /// Creates a new tooltip at the given position with the given pivot and the text of the tooltip with the given id.
    /// </summary>
    /// <param name="position">The position of the tooltip.</param>
    /// <param name="pivot">The pivot of the tooltip. Determines where the tooltip is anchored in relation to the position.</param>
    /// <param name="tooltipId">The id of the tooltip which text should be used.</param>
    /// <returns>The created tooltip component.</returns>
    public static ITooltipComponent ShowTooltipById(Vector2 position, TooltipPivot pivot, string tooltipId)
    {
        ArgumentNullException.ThrowIfNull(tooltipId);

        TooltipData? tooltipData = _dataProvider.GetTooltipData(tooltipId);
        if (tooltipData == null)
        { throw new ArgumentException($"No tooltip data found for id: {tooltipId}", nameof(tooltipId)); }

        // Create the tooltip and set its text.
        TooltipComponent tooltip = CreateTooltip();
        tooltip.Text = tooltipData.Text;

        // Calculate the position of the tooltip based on the pivot and the size of the tooltip.
        Vector2 placementPosition = CalculateNewTooltipLocation(position, pivot, tooltip.Size);
        placementPosition = CalculatePositionFromPivot(placementPosition, pivot, tooltip.Size);
        tooltip.Position = placementPosition;

        return tooltip;
    }

    #endregion API

    #region Utility Methods

    /// <summary>
    /// Calculates the position of a tooltip based on the size of the tooltip and the pivot.
    /// </summary>
    /// <remarks>
    /// Godot anchors and pivots don't work the same way as in Unity, so if we want to use the pivot as described int the remark of the <see cref="TooltipPivot"/> struct, we need to calculate the position of the tooltip based on its size and the pivot.
    /// </remarks>
    private static Vector2 CalculatePositionFromPivot(Vector2 position, TooltipPivot pivot, Vector2 tooltipSize)
    {
        GD.Print($"TODO: TooltipService: CalculatePositionFromPivot({position}, {pivot}, {tooltipSize})");
        return position;
    }

    /// <summary>
    /// Validates that the position for the new tooltip fits within the bounds of the screen and reuturns a fallback if it does not.
    /// Does not try to smartly position the tooltip, this method is for the case that the user has already supplied a position and we only need to check if it is valid!
    /// </summary>
    private static Vector2 CalculateNewTooltipLocation(Vector2 cursorPosition, TooltipPivot pivot, Vector2 size)
    {
        GD.Print($"TODO: TooltipService: CalculateNewTooltipLocation({cursorPosition}, {pivot}, {size})");
        return cursorPosition;
    }

    /// <summary>
    /// Calculates the position that a newly created, nested tooltip should be placed at, based on:<br/>
    /// - The position of the parent tooltip.<br/>
    /// - The position of the link that spawned the nested tooltip.<br/>
    /// - The cursor position.<br/>
    /// - The dimensions of the screen.<br/>
    /// </summary>
    private static (Vector2 position, TooltipPivot pivot) CalculateNestedTooltipLocation(ITooltipComponent tooltip, Vector2 cursorPosition)
    {
        GD.Print($"TODO: TooltipService: CalculateNestedTooltipLocation({tooltip}, {cursorPosition})");
        return (cursorPosition, TooltipPivot.BottomLeft);
    }

    private static TooltipComponent CreateTooltip()
    {
        string tooltipPrefabPath = TooltipPrefabPath ?? defaultTooltipPrefabPath;
        PackedScene tooltipScene = ResourceLoader.Load<PackedScene>(tooltipPrefabPath);

        if (tooltipScene == null)
        { throw new InvalidOperationException($"Could not load tooltip scene from path: {tooltipPrefabPath}"); }

        TooltipComponent tooltip = tooltipScene.Instantiate<TooltipComponent>();
        Instance._tooltipsParent.AddChild(tooltip);

        _activeTooltips.Add(tooltip, tooltip);
        return tooltip;
    }

    #endregion Utility Methods
}
