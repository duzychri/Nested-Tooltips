using Godot;
using NestedTooltips;
using System;

public partial class NestedTooltip : Button
{
    private ITooltip tooltip;
    [Export] private string tooltipId = "tooltip_normal_nesting";
    public override void _Ready()
    {
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExit;
    }

    private void OnMouseExit()
    {
        TooltipService.ReleaseTooltip(tooltip);
    }

    private void OnMouseEntered()
    {
        GD.Print($"{tooltipId}");
        Vector2 position = GetScreenPosition();
        tooltip = TooltipService.ShowTooltipById(position, TooltipPivot.BottomLeft, tooltipId);
    }
}
