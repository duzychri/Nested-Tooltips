// Created by: Christoph Duzy

namespace NestedTooltips;

/// <summary>
/// The information required to dispaly a tooltip based on it's id.
/// </summary>
public class TooltipData
{
    /// <summary>
    /// The unique identifier of the tooltip. Used to refence this tooltip from another nested tooltip.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// The bbcode formatted text of the tooltip.
    /// </summary>
    public required string Text { get; init; }

    /// <summary>
    /// If set, this determines the width of the tooltip in pixels.
    /// If no value is set then the tooltip will get as wide as it needs based on the text shown.
    /// This can get overwritten by the developer when calling the <see cref="TooltipService.ShowTooltip"/> methods and is mostly useful for nested tooltips.
    /// </summary>
    public int? DesiredWidth { get; init; } = null;
}
