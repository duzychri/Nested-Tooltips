namespace NestedTooltips.DemoScene;

public partial class Clamped : Button
{
    private ITooltip? _activeTooltip;
    private const string tooltipId = "tooltip_clamped";

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
        Vector2 position = new Vector2(-1000, -1000); // Place tooltip outside the screen bounds
        _activeTooltip = TooltipService.ShowTooltipById(position, TooltipPivot.BottomLeft, tooltipId);
    }
}
