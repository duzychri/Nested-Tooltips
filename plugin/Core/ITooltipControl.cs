namespace NestedTooltips;

public interface ITooltipControl
{
    /// <summary>
    /// Determines if the tooltip can be interacted with. This primarily determines if the tooltip should be passed up mouse events or not.
    /// </summary>
    bool IsInteractable { get; set; }

    /// <remarks>
    /// Sets the minimum width of the tooltip.
    /// </remarks>
    float MinimumWidth { get; set; }

    /// <summary>
    /// Shows the progress in time on how much longer the tooltip needs to stay open to be pinned.
    /// Ranges from 0 to 1, where 0 means no progress and 1 means the tooltip is pinned.
    /// </summary>
    double LockProgress { get; set; }

    /// <summary>
    /// Shows the progress in time on how much longer the tooltip needs to stay open to be unlocked.
    /// Ranges from 0 to 1, where 0 means no progress and 1 means the tooltip is unlocked.
    /// </summary>
    double UnlockProgress { get; set; }

    /// <summary>
    /// The text displayed in the tooltip.
    /// </summary>
    string Text { get; set; }

    /// <summary>
    /// Determines whether the text in the tooltip should wrap.
    /// </summary>
    bool WrapText { get; set; }

    /// <summary>
    /// Determines whether the tooltip is currently visible.
    /// </summary>
    bool Visible { get; set; }

    public Vector2 Size { get; set; }
    public Vector2 Position { get; set; }

    /// <summary>
    /// Event triggered when a link is clicked.
    /// </summary>
    event Action<Vector2, string>? OnLinkClicked;

    /// <summary>
    /// Event triggered when hovering over a link ends.
    /// </summary>
    event Action<Vector2, string>? OnLinkHoveredEnd;

    /// <summary>
    /// Event triggered when hovering over a link starts.
    /// </summary>
    event Action<Vector2, string>? OnLinkHoveredStart;

    void QueueFree();
    bool IsCursorOverTooltip();
}