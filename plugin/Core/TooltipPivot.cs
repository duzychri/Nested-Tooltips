namespace NestedTooltips;

/// <summary>
/// The pivot used to determine in what direction the tooltip should be 'drawn' in relation to the position supplied by the user.
/// The bottom left corner of the tooltip is represented by (0, 0), the top right corner by (1, 1).
/// </summary>
/// <remarks>
/// <b>For example:</b><br/>
///   - If the pivot is set to (0.5, 0.5) (the center) and placed at the mouse positon, then the the mouse will be exactly in the center of the tooltip.<br/>
///   - If the pivot is set to (0.0, 1.0) (the top left) and placed at the mouse position, then the tooltip will be below and right of the mouse position.<br/>
/// </remarks>
[DebuggerDisplay("{ToString()}")]
public readonly struct TooltipPivot
{
    #region Common Pivots

    /// <summary>
    /// Represents the position <c>(0.5, 0.5)</c>.
    /// </summary>
    public static readonly TooltipPivot Center = new(0.5f, 0.5f);
    /// <summary>
    /// Represents the position <c>(0.0, 1.0)</c>.
    /// </summary>
    public static readonly TooltipPivot TopLeft = new(0f, 1f);
    /// <summary>
    /// Represents the position <c>(1.0, 1.0)</c>.
    /// </summary>
    public static readonly TooltipPivot TopRight = new(1f, 1f);
    /// <summary>
    /// Represents the position <c>(0.0, 0.0)</c>.
    /// </summary>
    public static readonly TooltipPivot BottomLeft = new(0f, 0f);
    /// <summary>
    /// Represents the position <c>(1.0, 0.0)</c>.
    /// </summary>
    public static readonly TooltipPivot BottomRight = new(1f, 0f);

    #endregion Common Pivots

    /// <summary>
    /// The x coordinate of the anchor.
    /// </summary>
    public readonly float X;

    /// <summary>
    /// The y coordinate of the anchor.
    /// </summary>
    public readonly float Y;

    /// <summary>
    /// Initializes a new instance of the <see cref="TooltipPivot"/> struct with the specified x and y coordinates.
    /// </summary>
    /// <param name="x">The x coordinate of the anchor, clamped between 0 and 1.</param>
    /// <param name="y">The y coordinate of the anchor, clamped between 0 and 1.</param>
    public TooltipPivot(float x, float y)
    {
        X = Mathf.Clamp(x, 0f, 1f);
        Y = Mathf.Clamp(y, 0f, 1f);
    }

    #region Casts

    /// <summary>
    /// Implicitly converts a tuple of two floats to a <see cref="TooltipPivot"/> instance.
    /// </summary>
    /// <param name="tuple">A tuple containing the x and y coordinates.</param>
    /// <returns>A <see cref="TooltipPivot"/> instance initialized with the specified x and y coordinates.</returns>
    public static implicit operator TooltipPivot((float x, float y) tuple) => new(tuple.x, tuple.y);

    /// <summary>
    /// Implicitly converts a <see cref="TooltipPivot"/> instance to a tuple of two floats.
    /// </summary>
    /// <param name="pivot">The <see cref="TooltipPivot"/> instance to convert.</param>
    /// <returns>A tuple containing the x and y coordinates of the <see cref="TooltipPivot"/>.</returns>
    public static implicit operator (float x, float y)(TooltipPivot pivot) => (pivot.X, pivot.Y);

    #endregion Casts

    /// <inheritdoc />
    public override string ToString() => $"({X}, {Y})";
}