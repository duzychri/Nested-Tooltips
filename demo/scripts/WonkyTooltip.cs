namespace NestedTooltips.DemoScene;

public partial class WonkyTooltip : Button
{
    private List<ITooltip> _activeTooltips = new List<ITooltip>();
    private const string tooltipId = "tooltip_wonky_location";

    public override void _Ready()
    {
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExit;
    }

    private void OnMouseExit()
    {
        var activeTooltip = _activeTooltips[0];
        TooltipService.ReleaseTooltip(activeTooltip);
        _activeTooltips = new List<ITooltip>();
    }

    private void OnMouseEntered()
    {
        Vector2 position = new Vector2(0.0f, 0.0f);
        _activeTooltips.Add(TooltipService.ShowTooltipById(position, TooltipPivot.BottomLeft, tooltipId));
    }
}
