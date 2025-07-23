namespace NestedTooltips;

public partial class TooltipService : GodotSingelton<TooltipService>
{
    #region Classes

    private class TooltipHandler
    {
        private TooltipHandler? _child;
        private readonly TooltipHandler? _parent;
        private readonly ITooltipControl _control;

        /// <summary>The amount of time that the tooltip has been open.</summary>
        private double _aliveTime = 0.0;
        /// <summary>The time that the cursor has not been hovering over the tooltip.</summary>
        private double _cursorAwayTime = 0.0;
        /// <summary>Indicates if the tooltip is or was locked by a user interaction.</summary>
        private bool _wasLocked = false;
        /// <summary>Indicates that the tooltip should be destroyed under the right conditions.</summary>
        private bool _queuedForRelease = false;
        /// <summary>Shows that the tooltip has been deleted and should not be processed anymore.</summary>
        private bool _isFreed = false;

        public Tooltip Tooltip { get; private set; }
        public string Text { get => _control.Text; set => _control.Text = value; }
        public Vector2 Size { get => _control.Size; set => _control.Size = value; }
        public Vector2 Position { get => _control.Position; set => _control.Position = value; }

        public TooltipHandler(ITooltipControl control, TooltipHandler? parent)
        {
            _control = control;
            _parent = parent;

            Tooltip = new();
        }

        public void Release()
        {
            // Check if we are locked. If not, then we can just delete the tooltip immediately.
            if (_wasLocked == false)
            {
                Destroy();
                return;
            }

            _queuedForRelease = true;
        }

        public void ForceDestroy()
        {
            Destroy();
        }

        public void Process(double deltaTime)
        {
            // If the tooltip is already freed, we don't need to do anything.
            if (_isFreed)
            { return; }

            // Update how long the tooltip has been alive.
            _aliveTime += deltaTime;

            // Update how long the cursor has been away from the tooltip.
            if (_control.IsCursorOverTooltip() || _child != null)
            {
                _cursorAwayTime = 0.0;
                _control.UnlockProgress = 0.0;
            }
            else if (_queuedForRelease)
            {
                _cursorAwayTime += deltaTime;
                _control.UnlockProgress = GetUnlockProgress();
            }

            // Check if we are locked.
            switch (Settings.LockMode)
            {
                case TooltipLockMode.TimerLock:
                    {
                        (bool isLocked, double progress) = IsLockedByTimerLock();
                        _control.LockProgress = progress;
                        _wasLocked = _wasLocked || isLocked;
                    }
                    break;
                case TooltipLockMode.ActionLock:
                    {
                        bool isLocked = IsLockedByActionLock();
                        _wasLocked = _wasLocked || isLocked;
                        _control.LockProgress = isLocked ? 1.0 : 0.0;
                    }
                    break;
                case TooltipLockMode.MouseTendency:
                    {
                        bool isLocked = IsLockedByMouseTendency();
                        _wasLocked = _wasLocked || isLocked;
                        _control.LockProgress = isLocked ? 1.0 : 0.0;
                    }
                    break;
            }

            // Handle the release logic.
            if (_queuedForRelease)
            {
                // If we want to release the tooltip multiple conditions have to be fullfilled:
                // 1. The cursor can't be over the tooltip.
                // 2. The cursor has been away from the tooltip for a certain amount of time (unlocked).
                // 3. There can't be a child tooltip that is currently open.

                bool isCursorOverTooltip = _control.IsCursorOverTooltip();
                bool isUnlocked = GetUnlockProgress() >= 1;
                bool noOpenChild = _child == null;

                bool canRelease = isCursorOverTooltip == false && isUnlocked && noOpenChild;

                if (canRelease)
                {
                    Destroy();
                }
            }
        }

        private void Destroy()
        {
            _isFreed = true;
            _control.QueueFree();
        }

        #region Locking Logic

        private double GetUnlockProgress()
        {
            double unlockProgress = Math.Clamp(_cursorAwayTime / Settings.UnlockDelay, 0.0, 1.0);
            return unlockProgress;
        }

        private (bool isLocked, double progress) IsLockedByTimerLock()
        {
            bool isLocked = _aliveTime >= Settings.LockDelay;
            double lockProgress = Math.Clamp(_aliveTime / Settings.LockDelay, 0.0, 1.0);
            return (isLocked, lockProgress);
        }

        private bool IsLockedByActionLock()
        {
            return true;
        }

        private bool IsLockedByMouseTendency()
        {
            // Check the movement of the mouse over the last few frames and determine if it is moving towards the tooltip.
            return true;
        }

        #endregion Locking Logic
    }

    #endregion Classes

    private const string defaultTooltipPrefabPath = "res://demo/DemoTooltip.tscn";

    [Export] private Control _tooltipsParent = null!;

    private static readonly TooltipDataProvider _dataProvider = new();
    private static readonly Dictionary<ITooltip, TooltipHandler> _activeTooltips2 = [];

    #region Lifecycle Methods

    public override void _Process(double deltaTime)
    {
        // Naive implementation with ToArray, to avoid the source collection changing while iterating.
        // If this runs too slow we could use CopyTo with a buffer.
        foreach (TooltipHandler handlers in _activeTooltips2.Values.ToArray())
        {
            handlers.Process(deltaTime);
        }
    }

    #endregion Lifecycle Methods

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
    public static IEnumerable<ITooltip> ActiveTooltips => _activeTooltips2.Keys;

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
        (TooltipHandler handler, Tooltip tooltip) = CreateTooltip();
        handler.Text = text;

        // Calculate the position of the tooltip based on the pivot and the size of the tooltip.
        Vector2 placementPosition = CalculateNewTooltipLocation(position, pivot, handler.Size);
        placementPosition = CalculatePositionFromPivot(placementPosition, pivot, handler.Size);
        handler.Position = placementPosition;

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
        (TooltipHandler handler, Tooltip tooltip) = CreateTooltip();
        handler.Text = tooltipData.Text;

        // Calculate the position of the tooltip based on the pivot and the size of the tooltip.
        Vector2 placementPosition = CalculateNewTooltipLocation(position, pivot, handler.Size);
        placementPosition = CalculatePositionFromPivot(placementPosition, pivot, handler.Size);
        handler.Position = placementPosition;

        return tooltip;
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
        if (_activeTooltips2.TryGetValue(tooltip, out var handler) == false)
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
        if (_activeTooltips2.TryGetValue(tooltip, out TooltipHandler? handler) == false)
        {
            throw new ArgumentException($"Tooltip {tooltip} couldn't be found.", nameof(tooltip));
        }

        handler.Release();
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

    private static (TooltipHandler handler, Tooltip tooltip) CreateTooltip()
    {
        string tooltipPrefabPath = TooltipPrefabPath ?? defaultTooltipPrefabPath;
        PackedScene tooltipScene = ResourceLoader.Load<PackedScene>(tooltipPrefabPath);

        if (tooltipScene == null)
        { throw new InvalidOperationException($"Could not load tooltip scene from path: {tooltipPrefabPath}"); }

        TooltipControl tooltipControl = tooltipScene.Instantiate<TooltipControl>();
        Instance._tooltipsParent.AddChild(tooltipControl);

        TooltipHandler tooltipHandler = new(tooltipControl, null);

        Tooltip tooltip = tooltipHandler.Tooltip;
        _activeTooltips2.Add(tooltip, tooltipHandler);

        return (tooltipHandler, tooltip);
    }

    #endregion Utility Methods
}
