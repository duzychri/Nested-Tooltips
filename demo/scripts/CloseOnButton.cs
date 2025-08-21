using Godot;
using NestedTooltips;
using System;

public partial class CloseOnButton : Button
{
	private List<ITooltip> _activeTooltips = new List<ITooltip>();
	[Export] private string tooltipId = "tooltip_close_tooltip";
	public override void _Ready()
	{
		MouseEntered += OnMouseEntered;
		Pressed += OnPressed;
	}

	private void OnPressed()
	{
		TooltipService.ClearTooltips();
		_activeTooltips = new List<ITooltip>();
	}

	private void OnMouseEntered()
	{
		Vector2 position = GetScreenPosition();
		_activeTooltips.Add(TooltipService.ShowTooltipById(position, TooltipPivot.BottomLeft, tooltipId));
	}
}
