namespace NestedTooltips;

/// <summary>
/// Provides the <see cref="TooltipData"/> required for nested tooltips.
/// </summary>
public class TooltipDataProvider
{
    /// <summary>
    /// Finds and returns a <see cref="TooltipData"/> from its id.
    /// </summary>
    /// <param name="id">The unique identifier of the tooltip to load.</param>
    /// <returns>The <see cref="TooltipData"/> with the given id or <see langword="true"/> if nothing was found.</returns>
    public TooltipData? GetTooltipData(string id)
    {
        GD.PushWarning($"TODO: Implement {nameof(TooltipDataProvider)}.{nameof(GetTooltipData)}. Currently returning dummy data.");
        return new TooltipData() { Id = id, Name = "Dummy-Tooltip", Text = "This tooltip is a placeholder." };
    }
}
