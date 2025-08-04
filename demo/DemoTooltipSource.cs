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

    private ITooltip? _tooltipComponent;

    private void OnMouseEntered()
    {
        if (string.IsNullOrWhiteSpace(_tooltipText))
        {
            GD.PushWarning("No tooltip set!");
            return;
        }

        if (_tooltipComponent != null)
        {
            TooltipService.ReleaseTooltip(_tooltipComponent);
            _tooltipComponent = null;
        }

        // Get the absolute position of the label in the viewport.
        var labelPosition = GetScreenPosition();
        Vector2 tooltipPosition = labelPosition + new Vector2(0, -60);
        _tooltipComponent = TooltipService.ShowTooltip(tooltipPosition, (0, 0), _tooltipText);
    }

    private void OnMouseExited()
    {
        if (_tooltipComponent != null)
        {
            TooltipService.ReleaseTooltip(_tooltipComponent);
            _tooltipComponent = null;
        }
    }
}
