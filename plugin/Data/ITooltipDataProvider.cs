namespace NestedTooltips;

/// <summary>
/// Provides the <see cref="TooltipData"/> required for nested tooltips.
/// </summary>
public interface ITooltipDataProvider
{
    /// <summary>
    /// Finds and returns a <see cref="TooltipData"/> from its id.
    /// </summary>
    /// <param name="id">The unique identifier of the tooltip to load.</param>
    /// <returns>The <see cref="TooltipData"/> with the given id or <see langword="true"/> if nothing was found.</returns>
    TooltipData? GetTooltipData(string id);
}