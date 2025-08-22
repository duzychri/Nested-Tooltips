using Godot;

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
    /// The position that the tooltip is displayed at on the screen.
    /// </summary>
    public Vector2 Position { get; }
    /// <summary>
    /// The pivot point of the tooltip, which determines how the tooltip is positioned relative to its <see cref="Position"/>.
    /// </summary>
    public TooltipPivot Pivot { get; }
}
