namespace NestedTooltips.DemoScene;

public partial class CloseOnButton : Button
{
    private ITooltip? _activeTooltip;
    private const string tooltipId = "tooltip_close_tooltip";

    public override void _Ready()
    {
        MouseEntered += OnMouseEntered;
        Pressed += OnPressed;
    }

    private void OnPressed()
    {
        TooltipService.ClearTooltips();
        _activeTooltip = null;
    }

    private void OnMouseEntered()
    {
        Vector2 position = GetScreenPosition();
        _activeTooltip = TooltipService.ShowTooltipById(position, TooltipPivot.BottomLeft, tooltipId);
    }
}
