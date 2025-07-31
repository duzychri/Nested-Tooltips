namespace NestedTooltips.DemoScene;

public partial class DemoSceneManager : Node
{
    [Export] private string _text = "Hello World!";
    [Export] private RichTextLabel _demoTextLabel = null!;

    public override void _Ready()
    {
        _demoTextLabel.MetaHoverStarted += OnMetaHoveredStart;
        _demoTextLabel.MetaHoverEnded += OnMetaHoveredEnd;
        _demoTextLabel.MetaClicked += OnMetaClicked;
        GD.Print(_text);
    }

    private ITooltip? _tooltipComponent;

    private void OnMetaHoveredStart(Variant meta)
    {
        var tooltipTextId = meta.AsString();
        var mousePosition = GetViewport().GetMousePosition();
        _tooltipComponent = TooltipService.ShowTooltipById(mousePosition, (0, 0), tooltipTextId);
    }

    private void OnMetaHoveredEnd(Variant meta)
    {
        if (_tooltipComponent != null)
        {
            TooltipService.ReleaseTooltip(_tooltipComponent);
            _tooltipComponent = null;
        }
    }

    private void OnMetaClicked(Variant meta)
    {
        if (_tooltipComponent != null)
        {
            TooltipService.ActionLockTooltip(_tooltipComponent);
        }
    }
}
