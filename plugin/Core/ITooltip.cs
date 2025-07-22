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
}
