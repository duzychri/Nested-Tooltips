using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

using NestedTooltips.Internals;

namespace NestedTooltips;

/// <summary>
/// Service for managing tooltips in the application.
/// </summary>
public partial class TooltipService : GodotSingelton<TooltipService>
{
    [Export] private Control _tooltipsParent = null!;

    private const string defaultTooltipPrefabPath = "res://addons/nested_tooltips/Prefabs/DefaultTooltip_Prefab.tscn";

    private static readonly HashSet<ITooltip> _destroyedTooltips = [];
    private static readonly Dictionary<ITooltip, TooltipHandler> _activeTooltips = [];

    #region Lifecycle Methods

    public override void _Process(double deltaTime)
    {
        // Naive implementation with ToArray, to avoid the source collection changing while iterating.
        // If this runs too slow we could use CopyTo with a buffer.
        foreach (TooltipHandler handlers in _activeTooltips.Values.ToArray())
        {
            handlers.Process(deltaTime);
        }

        // Destroy all tooltips that were queued for destruction.
        // We do this so we don't brick the IEnumerable returned by the ActiveTooltips property (assuming the developer hasn't copied the collection).
        foreach (ITooltip tooltip in _destroyedTooltips.ToArray())
        {
            _activeTooltips.Remove(tooltip);
        }
        _destroyedTooltips.Clear();
    }

    #endregion Lifecycle Methods

    #region Configuration API

    private static ITooltipDataProvider _dataProvider = BasicTooltipDataProvider.Empty;

    /// <summary>
    /// The provider used to retrieve tooltip data by id.
    /// By default this is set to an empty provider that returns null for all ids.
    /// </summary>
    public static ITooltipDataProvider TooltipDataProvider
    {
        get => _dataProvider;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            _dataProvider = value;

            // Destroy all currently active tooltips.
            ClearAllTooltips();
        }
    }

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

            Node instance = scene.Instantiate();
            if (instance is not ITooltipControl)
            {
                // Clean up the instance to avoid memory leaks.
                instance.QueueFree();
                throw new InvalidOperationException($"The tooltip prefab at '{value}' does not implement ITooltipControl.");
            }

            // Clean up the instance to avoid memory leaks.
            instance.QueueFree();

            // If all checks pass, set the path.
            _tooltipPrefabPath = value;

            // Destroy all currently active tooltips.
            ClearAllTooltips();
        }
    }

    private static TooltipSettings _settings = TooltipSettings.Default;

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
            return _settings;
        }
        set
        {
            _settings = value;
            ClearAllTooltips();
        }
    }

    #endregion Configuration API

    #region API

    /// <summary>
    /// All currently active tooltips, including all nested ones.
    /// </summary>
    public static IEnumerable<ITooltip> ActiveTooltips => _activeTooltips.Keys.Where(t => _destroyedTooltips.Contains(t) == false);

    /// <summary>
    /// Creates a new tooltip at the given position with the given pivot and text.
    /// </summary>
    /// <param name="position">The position of the tooltip.</param>
    /// <param name="pivot">The pivot of the tooltip. Determines where the tooltip is anchored in relation to the position.</param>
    /// <param name="text">The bbcode formatted text of the tooltip.</param>
    /// <param name="width">The desired width of the tooltip in pixels. If not set, the tooltip will be as wide as it needs to be based on the text.</param>
    /// <returns>The created tooltip component.</returns>
    public static ITooltip ShowTooltip(Vector2 position, TooltipPivot pivot, string text, int? width = null)
    {
        ArgumentNullException.ThrowIfNull(text);

        // Create the tooltip and set its text.
        (TooltipHandler handler, ITooltip tooltip) = CreateTooltip(position, pivot, width);
        handler.Text = text;

        return tooltip;
    }

    /// <summary>
    /// Creates a new tooltip at the given position with the given pivot and the text of the tooltip with the given id.
    /// </summary>
    /// <param name="position">The position of the tooltip.</param>
    /// <param name="pivot">The pivot of the tooltip. Determines where the tooltip is anchored in relation to the position.</param>
    /// <param name="tooltipId">The id of the tooltip which text should be used.</param>
    /// <param name="width">The desired width of the tooltip in pixels. If not set, the tooltip will be as wide as it needs to be based on the text.</param>
    /// <returns>The created tooltip component.</returns>
    public static ITooltip ShowTooltipById(Vector2 position, TooltipPivot pivot, string tooltipId, int? width = null)
    {
        ArgumentNullException.ThrowIfNull(tooltipId);

        TooltipData? tooltipData = _dataProvider.GetTooltipData(tooltipId);
        if (tooltipData == null)
        { throw new ArgumentException($"No tooltip data found for id: {tooltipId}", nameof(tooltipId)); }

        // Create the tooltip and set its text.
        (TooltipHandler handler, ITooltip tooltip) = CreateTooltip(position, pivot, width ?? tooltipData.DesiredWidth);
        handler.Text = tooltipData.Text;

        return tooltip;
    }

    /// <summary>
    /// If <see cref="TooltipLockMode.ActionLock"/> is set in the settings, then this method will lock a tooltip created by a <see cref="ShowTooltip"/> method.
    /// </summary>
    /// <param name="tooltip">The tooltip to lock.</param>
    public static void ActionLockTooltip(ITooltip tooltip)
    {
        ArgumentNullException.ThrowIfNull(tooltip);

        // Check if the tooltip exists.
        if (_activeTooltips.TryGetValue(tooltip, out TooltipHandler? handler) == false)
        {
            throw new ArgumentException($"Tooltip {tooltip} couldn't be found.", nameof(tooltip));
        }

        // Lock the tooltip.
        handler.OnThisClicked();
    }

    /// <summary>
    /// Forces the tooltip all nested tooltips to be removed.
    /// Ignores the locking behaviour of the tooltip and will destroy the tooltip even if it is locked.
    /// </summary>
    /// <param name="tooltip">The tooltip to destroy.</param>
    public static void ForceDestroy(ITooltip tooltip)
    {
        ArgumentNullException.ThrowIfNull(tooltip);

        // Check if the tooltip exists.
        if (_activeTooltips.TryGetValue(tooltip, out TooltipHandler? handler) == false)
        {
            throw new ArgumentException($"Tooltip {tooltip} couldn't be found.", nameof(tooltip));
        }

        handler.ForceDestroy();
    }

    /// <summary>
    /// Sets a flag on the tooltip that indicates to the tooltip that it can destroy itself if the locking behaviour allows it.
    /// Use this if the source that created the tooltip no longer desires the tooltip to be shown, for example if the user stopped hovering over the button that caused the tooltip to open.
    /// </summary>
    /// <param name="tooltip">Tooltip to set the releasable flag on.</param>
    public static void ReleaseTooltip(ITooltip tooltip)
    {
        ArgumentNullException.ThrowIfNull(tooltip);

        // Check if tooltip exists.
        if (_activeTooltips.TryGetValue(tooltip, out TooltipHandler? handler) == false)
        {
            throw new ArgumentException($"Tooltip {tooltip} couldn't be found.", nameof(tooltip));
        }

        handler.Release();
    }

    /// <summary>
    /// Forcefully destroys all exiting tooltips.
    /// </summary>
    public static void ClearTooltips()
    {
        ClearAllTooltips();
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
        return position - (tooltipSize * (Vector2)pivot);
    }

    /// <summary>
    /// Validates that the position for the new tooltip fits within the bounds of the screen and returns a fallback if it does not.
    /// Does not try to smartly position the tooltip, this method is for the case that the user has already supplied a position and we only need to check if it is valid!
    /// </summary>
    private static Vector2 CalculateNewTooltipLocation(Vector2 position, Vector2 size)
    {
        // Get the current viewport size to handle screen resizing.
        Vector2I viewportSize = DisplayServer.WindowGetSize();

        // Clamp the position to ensure the entire tooltip stays within the screen bounds.
        float clampedX = Mathf.Clamp(position.X, 0, Mathf.Max(0, viewportSize.X - size.X));
        float clampedY = Mathf.Clamp(position.Y, 0, Mathf.Max(0, viewportSize.Y - size.Y));

        return new Vector2(clampedX, clampedY);
    }

    /// <summary>
    /// Calculates the position that a newly created, nested tooltip should be placed at, based on:<br/>
    /// - The position of the parent tooltip.<br/>
    /// - The cursor position.<br/>
    /// - The dimensions of the screen.<br/>
    /// </summary>
    private static (Vector2 position, TooltipPivot pivot) CalculateNestedTooltipLocation(Vector2 position)
    {
        const float offsetFactor = 15f; // Offset factor to avoid overlap with the cursor.

        // Get the current viewport size and its center to determine screen quadrants.
        Vector2I viewportSize = DisplayServer.WindowGetSize(); /// correct way to find out the size of the viewport like in the example for multipleTooltips?
        Vector2 screenCenter = (Vector2)viewportSize / 2f;

        TooltipPivot pivot;
        // Offset to avoid overlap with the cursor.
        Vector2 offset = Vector2.Zero;

        // Determine the best pivot based on which screen quadrant the cursor is in.
        // This places the tooltip in the direction with the most available space.
        if (position.X < screenCenter.X) // Cursor is on the left half of the screen
        {
            if (position.Y < screenCenter.Y) // Top-Left Quadrant -> open bottom-right
            {
                pivot = TooltipPivot.TopLeft;
                offset = new Vector2(+1, +1) * offsetFactor;
            }
            else // Bottom-Left Quadrant -> open top-right
            {
                pivot = TooltipPivot.BottomLeft;
                offset = new Vector2(+1, -1) * offsetFactor;
            }
        }
        else // Cursor is on the right half of the screen
        {
            if (position.Y < screenCenter.Y) // Top-Right Quadrant -> open bottom-left
            {
                pivot = TooltipPivot.TopRight;
                offset = new Vector2(-1, +1) * offsetFactor;
            }
            else // Bottom-Right Quadrant -> open top-left
            {
                pivot = TooltipPivot.BottomRight;
                offset = new Vector2(-1, -1) * offsetFactor;
            }
        }
        position += offset;

        return (position, pivot);
    }

    private static (TooltipHandler handler, ITooltip tooltip) CreateTooltip(Vector2 position, TooltipPivot pivot, int? width, TooltipHandler? parentHandler = null)
    {
        string tooltipPrefabPath = TooltipPrefabPath ?? defaultTooltipPrefabPath;
        PackedScene tooltipScene = ResourceLoader.Load<PackedScene>(tooltipPrefabPath);

        if (tooltipScene == null)
        { throw new InvalidOperationException($"Could not load tooltip scene from path: {tooltipPrefabPath}"); }

        Node tooltipControlNode = tooltipScene.Instantiate();
        Instance._tooltipsParent.AddChild(tooltipControlNode);

        if (tooltipControlNode is not ITooltipControl tooltipControl)
        { throw new InvalidOperationException($"The tooltip prefab at '{tooltipPrefabPath}' does not implement ITooltipControl."); }

        TooltipHandler tooltipHandler = new(tooltipControl, parentHandler, position, pivot, width);

        ITooltip tooltip = tooltipHandler.Tooltip;
        _activeTooltips.Add(tooltip, tooltipHandler);

        return (tooltipHandler, tooltip);
    }

    private static void ClearAllTooltips()
    {
        // Clear all active tooltips.
        foreach (TooltipHandler handler in _activeTooltips.Values.ToArray())
        {
            handler.ForceDestroy();
        }
    }

    #endregion Utility Methods
}
