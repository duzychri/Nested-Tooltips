namespace NestedTooltips.DemoScene;

[Tool]
public partial class DemoSlider : HBoxContainer
{
    [ExportCategory("Configuration")]
    [Export] private string _name = null!;
    [Export(PropertyHint.MultilineText)] private string _tooltipText = null!;

    [ExportCategory("Components")]
    [Export] private Label _labelName = null!;
    [Export] private Label _labelAmount = null!;
    [Export] private HSlider _slider = null!;

    public override void _Ready()
    {
        // This checks if we are in the editor, because this is a Tool script that will be executed in the editor.
        if (Engine.IsEditorHint())
        { return; }

        // Create callback when the mouse hovers over _labelName.
        _labelName.MouseEntered += OnMouseEntered;
        _labelName.MouseExited += OnMouseExited;
    }

    public override void _Process(double delta)
    {
        // This checks if we are in the editor, because this is a Tool script that will be executed in the editor.
        if (Engine.IsEditorHint())
        {
            _labelName.Text = _name;
            return;
        }
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
            _tooltipComponent.SetReleasable(true);
            _tooltipComponent = null;
        }

        // Get the absolute position of the label in the viewport.
        var labelPosition = _labelName.GetScreenPosition();
        Vector2 tooltipPosition = labelPosition + new Vector2(0, -60);
        _tooltipComponent = TooltipService.ShowTooltip(tooltipPosition, (0, 0), _tooltipText);
    }

    private void OnMouseExited()
    {
        if (_tooltipComponent != null)
        {
            _tooltipComponent.SetReleasable(true);
            _tooltipComponent = null;
        }
    }
}
