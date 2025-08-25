// Created by: Christoph Duzy

using Godot;
using System;

namespace NestedTooltips;

/// <summary>
/// The readonly class that represents a created tooltip.
/// </summary>
public class Tooltip : ITooltip
{
    /// <inheritdoc />
    public ITooltip? Parent { get; }
    /// <inheritdoc />
    public ITooltip? Child => _getChild.Invoke();
    /// <inheritdoc />
    public string Text { get; } = "";
    /// <inheritdoc />
    public Vector2 Position { get; }
    /// <inheritdoc />
    public TooltipPivot Pivot { get; }

    private readonly Func<ITooltip?> _getChild;

    internal Tooltip(Func<ITooltip?> getChild, ITooltip? parent, string text, Vector2 position, TooltipPivot pivot)
    {
        ArgumentNullException.ThrowIfNull(getChild);

        _getChild = getChild;
        Parent = parent;
        Text = text;
        Position = position;
        Pivot = pivot;
    }
}