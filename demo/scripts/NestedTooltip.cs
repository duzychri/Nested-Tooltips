namespace NestedTooltips.DemoScene;

public partial class NestedTooltip : Button
{
    private ITooltip? _activeTooltip;
    private const string tooltipId = "tooltip_normal_nesting";

    public override void _Ready()
    {
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
    }

    private void OnMouseExited()
    {
        if (_activeTooltip != null)
        {
            TooltipService.ReleaseTooltip(_activeTooltip);
            _activeTooltip = null;
        }
    }

    private void OnMouseEntered()
    {
        Vector2 position = GetScreenPosition();
        _activeTooltip = TooltipService.ShowTooltipById(position, TooltipPivot.BottomLeft, tooltipId);
    }
}
