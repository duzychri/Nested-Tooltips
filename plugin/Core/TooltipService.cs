namespace NestedTooltips;

public partial class TooltipService : GodotSingelton<TooltipService>
{
    /// <summary>
    /// All currently active tooltips, including all nested ones.
    /// </summary>
    public static IEnumerable<ITooltipComponent> ActiveTooltips { get; } = [];

    /// <summary>
    /// Creates a new tooltip at the given position with the given pivot and text.
    /// </summary>
    /// <param name="position">The position of the tooltip.</param>
    /// <param name="pivot">The pivot of the tooltip. Determines where the tooltip is anchored in relation to the position.</param>
    /// <param name="text">The bbcode formatted text of the tooltip.</param>
    /// <returns>The created tooltip component.</returns>
    public static ITooltipComponent ShowTooltip((int x, int y) position, TooltipPivot pivot, string text)
    {
        GD.Print($"TODO: TooltipService: ShowTooltip({position}, {pivot}, {text})");
        return new TooltipComponent();
    }

    /// <summary>
    /// Creates a new tooltip at the given position with the given pivot and the text of the tooltip with the given id.
    /// </summary>
    /// <param name="position">The position of the tooltip.</param>
    /// <param name="pivot">The pivot of the tooltip. Determines where the tooltip is anchored in relation to the position.</param>
    /// <param name="tooltipId">The id of the tooltip which text should be used.</param>
    /// <returns>The created tooltip component.</returns>
    public static ITooltipComponent ShowTooltipById((int x, int y) position, TooltipPivot pivot, string tooltipId)
    {
        GD.Print($"TODO: TooltipService: ShowTooltipById({position}, {pivot}, {tooltipId})");
        return new TooltipComponent();
    }

    /// <summary>
    /// Validates that the position for the new tooltip fits within the bounds of the screen and reuturns a fallback if it does not.
    /// </summary>
    private static ((int x, int y) position, TooltipPivot pivot) CalculateNewTooltipLocation((int x, int y) cursorPosition, TooltipPivot pivot)
    {
        GD.Print($"TODO: TooltipService: CalculateNewTooltipLocation({cursorPosition}, {pivot})");
        return (cursorPosition, TooltipPivot.BottomLeft);
    }

    /// <summary>
    /// Calculates the position that a newly created, nested tooltip should be placed at, based on:<br/>
    /// - The position of the parent tooltip.<br/>
    /// - The position of the link that spawned the nested tooltip.<br/>
    /// - The cursor position.<br/>
    /// - The dimensions of the screen.<br/>
    /// </summary>
    private static ((int x, int y) position, TooltipPivot pivot) CalculateNestedTooltipLocation(ITooltipComponent tooltip, (int x, int y) cursorPosition)
    {
        GD.Print($"TODO: TooltipService: CalculateNestedTooltipLocation({tooltip}, {cursorPosition})");
        return (cursorPosition, TooltipPivot.BottomLeft);
    }
}
