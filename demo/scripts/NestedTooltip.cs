using Godot;
using NestedTooltips;
using System;

public partial class NestedTooltip : Button
{
    private List<ITooltip> _activeTooltips = new List<ITooltip>();
    [Export] private string tooltipId = "tooltip_normal_nesting";
    public override void _Ready()
    {
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExit;
    }

    private void OnMouseExit()
    {
        var activeTooltip = _activeTooltips[0];
		TooltipService.ReleaseTooltip(activeTooltip);
		_activeTooltips = new List<ITooltip>();
    }

    private void OnMouseEntered()
    {
        Vector2 position = GetScreenPosition();
        _activeTooltips.Add(TooltipService.ShowTooltipById(position, TooltipPivot.BottomLeft, tooltipId));
    }
}
