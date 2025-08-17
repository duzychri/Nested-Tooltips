using Godot;
using NestedTooltips;
using System;

public partial class WonkyTooltip : Button
{
    private ITooltip tooltip;
    [Export] private string tooltipId = "tooltip_wonky_location";
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
        Vector2 position = new Vector2(0.0f, 0.0f);
        tooltip = TooltipService.ShowTooltipById(position, TooltipPivot.TopLeft, tooltipId);
    }
}
