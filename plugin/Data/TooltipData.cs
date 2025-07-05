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
    /// The display name of the tooltip. Is inserted into a tooltip text if the tooltip is referenced there and didn't get a custom name assigned.
    /// </summary>
    public required string Name { get; init; }
    /// <summary>
    /// The bbcode formatted text of the tooltip.
    /// </summary>
    public required string Text { get; init; }
}
