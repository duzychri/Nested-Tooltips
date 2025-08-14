namespace NestedTooltips;

public partial class TooltipService : GodotSingelton<TooltipService>
{
    [Export] private Control _tooltipsParent = null!;

    private const string defaultTooltipPrefabPath = "res://demo/DemoTooltip.tscn";

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
            if (_activeTooltips.ContainsKey(tooltip))
            { _activeTooltips.Remove(tooltip); }
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
    /// <returns>The created tooltip component.</returns>
    public static ITooltip ShowTooltip(Vector2 position, TooltipPivot pivot, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        // Create the tooltip and set its text.
        (TooltipHandler handler, Tooltip tooltip) = CreateTooltip(position, pivot);
        handler.Text = text;

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
        (TooltipHandler handler, Tooltip tooltip) = CreateTooltip(position, pivot);
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

    private static (TooltipHandler handler, Tooltip tooltip) CreateTooltip(Vector2 position, TooltipPivot pivot, TooltipHandler? parentHandler = null)
    {
        string tooltipPrefabPath = TooltipPrefabPath ?? defaultTooltipPrefabPath;
        PackedScene tooltipScene = ResourceLoader.Load<PackedScene>(tooltipPrefabPath);

        if (tooltipScene == null)
        { throw new InvalidOperationException($"Could not load tooltip scene from path: {tooltipPrefabPath}"); }

        Node tooltipControlNode = tooltipScene.Instantiate();
        Instance._tooltipsParent.AddChild(tooltipControlNode);

        ITooltipControl? tooltipControl = tooltipControlNode as ITooltipControl;
        if (tooltipControl == null)
        { throw new InvalidOperationException($"The tooltip prefab at '{tooltipPrefabPath}' does not implement ITooltipControl."); }

        TooltipHandler tooltipHandler = new(tooltipControl, parentHandler, position, pivot);

        Tooltip tooltip = tooltipHandler.Tooltip;
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
