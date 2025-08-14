namespace NestedTooltips.DemoScene;

public partial class DemoTooltipSource : Control
{
    [ExportCategory("Configuration")]
    [Export(PropertyHint.MultilineText)] private string _tooltipText = null!;

    public override void _Ready()
    {
        // Don't create a tooltip if we don't have any text to show.
        if (string.IsNullOrWhiteSpace(_tooltipText))
        { return; }

        // Create callback when the mouse hovers over _labelName.
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
    }

    private ITooltip? _tooltip;

    private void OnMouseEntered()
    {
        if (string.IsNullOrWhiteSpace(_tooltipText))
        {
            GD.PushWarning("No tooltip set!");
            return;
        }

        if (_tooltip != null)
        {
            TooltipService.ReleaseTooltip(_tooltip);
            _tooltip = null;
        }

        // Get the absolute position of the label in the viewport.
        Vector2 labelPosition = GetScreenPosition();
        Vector2 tooltipPosition = labelPosition;
        _tooltip = TooltipService.ShowTooltip(tooltipPosition, TooltipPivot.BottomLeft, _tooltipText);
        TooltipService.ActionLockTooltip(_tooltip);
    }

    private void OnMouseExited()
    {
        if (_tooltip != null)
        {
            TooltipService.ReleaseTooltip(_tooltip);
            _tooltip = null;
        }
    }
}
