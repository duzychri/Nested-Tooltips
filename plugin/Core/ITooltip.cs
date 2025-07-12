namespace NestedTooltips;

/// <summary>
/// Interface for a tooltip component that can be nested within other tooltips.
/// </summary>
public interface ITooltip
{
    /// <summary>
    /// The parent tooltip of this tooltip. <see langword="null"/> if this tooltip is a root tooltip.
    /// </summary>
    public ITooltip? Parent { get; }
    /// <summary>
    /// The nested tooltip spawned from this tooltip. <see langword="null"/> if this tooltip has no child.
    /// </summary>
    public ITooltip? Child { get; }
    /// <summary>
    /// The bbcode formatted text of the tooltip.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Event that is called after this tooltip is destroyed.
    /// </summary>
    public event Action? OnTooltipDestroyed;

    /// <summary>
    /// Forces the tooltip all nested tooltips to be removed.
    /// Ignores the locking behaviour of the tooltip and will destroy the tooltip even if it is locked.
    /// </summary>
    void ForceDestroy();

    /// <summary>
    /// Sets a flag on the tooltip that indicates to the tooltip that it can destroy itself if the locking behaviour allows it.
    /// Use this if the source that created the tooltip no longer desires the tooltip to be shown, for example if the user stopped hovering over the button that caused the tooltip to open.
    /// </summary>
    /// <param name="value"><see langword="true"/> if the tooltip can be destroyed, <see langword="false"/> if it should not be destroyed.</param>
    void SetReleasable(bool value);
}
