namespace NestedTooltips;

/// <summary>
/// Provides the <see cref="TooltipData"/> required for nested tooltips.
/// </summary>
public class TooltipDataProvider : ITooltipDataProvider
{
    /// <inheritdoc />
    public TooltipData? GetTooltipData(string id)
    {
        GD.PushWarning($"TODO: Implement {nameof(TooltipDataProvider)}.{nameof(GetTooltipData)}. Currently returning dummy data.");
        return new TooltipData() { Id = id, Name = "Dummy-Tooltip", Text = "[p]This tooltip is a placeholder.[/p][p]It also itself also contains another [url=tooltip_demo_id_2}]link[/url] to a nested tooltip.[/p]" };
    }
}
