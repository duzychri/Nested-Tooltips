using System.IO;

namespace NestedTooltips;

public partial class TooltipService : GodotSingelton<TooltipService>
{
    private const string defaultTooltipPrefabPath = "res://demo/DemoTooltip.tscn";

    [Export] private Control _tooltipsParent = null!;

    private static readonly TooltipDataProvider _dataProvider = new();
    private static readonly Dictionary<ITooltip, TooltipControl> _activeTooltips = [];

    #region Configuration API

    private static string? _tooltipPrefabPath = null;

    /// <summary>
    /// The path to the tooltip prefab that is used to create new tooltips.
    /// Set this if you want to use a different tooltip prefab than the default one.
    /// </summary>
    public static string? TooltipPrefabPath
    {
        get
        {
            return _tooltipPrefabPath;
        }
        set
        {
            // Check if the path leads to a resource that exists.
            if (ResourceLoader.Exists(value) == false)
            {
                throw new InvalidOperationException($"The tooltip prefab path '{value}' does not point to a valid resource.");
            }

            // Check that the resource instantiates a TooltipControl or ITooltipControl.
            PackedScene? scene = ResourceLoader.Load<PackedScene>(value);
            if (scene == null)
            {
                throw new InvalidOperationException($"The tooltip prefab path '{value}' does not point to a valid PackedScene.");
            }

            if (scene.Instantiate() is not ITooltipControl)
            {
                throw new InvalidOperationException($"The tooltip prefab at '{value}' does not implement ITooltipControl.");
            }

            // If all checks pass, set the path.
            _tooltipPrefabPath = value;
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

    #endregion Configuration API

    #region API

    /// <summary>
    /// All currently active tooltips, including all nested ones.
    /// </summary>
    public static IEnumerable<ITooltip> ActiveTooltips => _activeTooltips.Keys;

    /// <summary>
    /// Creates a new tooltip at the given position with the given pivot and text.
    /// </summary>
    /// <param name="position">The position of the tooltip.</param>
    /// <param name="pivot">The pivot of the tooltip. Determines where the tooltip is anchored in relation to the position.</param>
    /// <param name="text">The bbcode formatted text of the tooltip.</param>
    /// <returns>The created tooltip component.</returns>
    public static ITooltip ShowTooltip(Vector2 position, TooltipPivot pivot, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        // Create the tooltip and set its text.
        Tooltip tooltip = CreateTooltip();
        tooltip.Control.Text = text;

        // Calculate the position of the tooltip based on the pivot and the size of the tooltip.
        Vector2 placementPosition = CalculateNewTooltipLocation(position, pivot, tooltip.Control.Size);
        placementPosition = CalculatePositionFromPivot(placementPosition, pivot, tooltip.Control.Size);
        tooltip.Control.Position = placementPosition;

        return tooltip;
    }

    /// <summary>
    /// Creates a new tooltip at the given position with the given pivot and the text of the tooltip with the given id.
    /// </summary>
    /// <param name="position">The position of the tooltip.</param>
    /// <param name="pivot">The pivot of the tooltip. Determines where the tooltip is anchored in relation to the position.</param>
    /// <param name="tooltipId">The id of the tooltip which text should be used.</param>
    /// <returns>The created tooltip component.</returns>
    public static ITooltip ShowTooltipById(Vector2 position, TooltipPivot pivot, string tooltipId)
    {
        ArgumentNullException.ThrowIfNull(tooltipId);

        TooltipData? tooltipData = _dataProvider.GetTooltipData(tooltipId);
        if (tooltipData == null)
        { throw new ArgumentException($"No tooltip data found for id: {tooltipId}", nameof(tooltipId)); }

        // Create the tooltip and set its text.
        Tooltip tooltip = CreateTooltip();
        tooltip.Control.Text = tooltipData.Text;

        // Calculate the position of the tooltip based on the pivot and the size of the tooltip.
        Vector2 placementPosition = CalculateNewTooltipLocation(position, pivot, tooltip.Control.Size);
        placementPosition = CalculatePositionFromPivot(placementPosition, pivot, tooltip.Control.Size);
        tooltip.Control.Position = placementPosition;

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
    private static (Vector2 position, TooltipPivot pivot) CalculateNestedTooltipLocation(TooltipControl control, Vector2 cursorPosition)
    {
        GD.Print($"TODO: TooltipService: CalculateNestedTooltipLocation({control}, {cursorPosition})");
        return (cursorPosition, TooltipPivot.BottomLeft);
    }

    private static Tooltip CreateTooltip()
    {
        string tooltipPrefabPath = TooltipPrefabPath ?? defaultTooltipPrefabPath;
        PackedScene tooltipScene = ResourceLoader.Load<PackedScene>(tooltipPrefabPath);

        if (tooltipScene == null)
        { throw new InvalidOperationException($"Could not load tooltip scene from path: {tooltipPrefabPath}"); }

        TooltipControl tooltipControl = tooltipScene.Instantiate<TooltipControl>();
        Instance._tooltipsParent.AddChild(tooltipControl);

        Tooltip tooltip = new(tooltipControl);
        _activeTooltips.Add(tooltip, tooltipControl);

        return tooltip;
    }

    #endregion Utility Methods
}
