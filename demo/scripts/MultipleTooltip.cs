using Godot;
using NestedTooltips;
using System;
using System.Numerics;

public partial class MultipleTooltip : Button
{
	private ITooltip tooltip;
    private ITooltip tooltip1;
    private ITooltip tooltip2;
    private ITooltip tooltip3;
    private ITooltip tooltip4;
	[Export] private string tooltipIdMain = "tooltip_multiple";
	[Export] private string tooltipId1 = "tooltip_multiple_top_right";
	[Export] private string tooltipId2 = "tooltip_multiple_top_left";
	[Export] private string tooltipId3 = "tooltip_multiple_bottom_right";
	[Export] private string tooltipId4 = "tooltip_multiple_bottom_left";
	public override void _Ready()
	{
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExit;
	}

    private void OnMouseExit()
    {
        TooltipService.ReleaseTooltip(tooltip);
        TooltipService.ReleaseTooltip(tooltip1);
        TooltipService.ReleaseTooltip(tooltip2);
        TooltipService.ReleaseTooltip(tooltip3);
        TooltipService.ReleaseTooltip(tooltip4);
	}

	private void OnMouseEntered()
	{
		GD.Print($"{tooltipIdMain}");
		Godot.Vector2 position = GetScreenPosition();
		tooltip = TooltipService.ShowTooltipById(position, TooltipPivot.BottomLeft, tooltipIdMain);
		Godot.Vector2 position1 = new Godot.Vector2(1152.0f, 0.0f);
		tooltip1 = TooltipService.ShowTooltipById(position1, TooltipPivot.TopRight, tooltipId1);
		GD.Print($"{tooltipId1}");
		Godot.Vector2 position2 = new Godot.Vector2(1152.0f, 648.0f);
		tooltip2 = TooltipService.ShowTooltipById(position2, TooltipPivot.BottomRight, tooltipId2);
		GD.Print($"{tooltipId2}");
		Godot.Vector2 position3 = new Godot.Vector2(0.0f, 0.0f);
		tooltip3 = TooltipService.ShowTooltipById(position3, TooltipPivot.TopLeft, tooltipId3);
		GD.Print($"{tooltipId3}");
		Godot.Vector2 position4 = new Godot.Vector2(0.0f, 648.0f);
		tooltip4 = TooltipService.ShowTooltipById(position4, TooltipPivot.BottomLeft, tooltipId4);
		GD.Print($"{tooltipId4}");
	}
}
