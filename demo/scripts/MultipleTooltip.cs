namespace NestedTooltips.DemoScene;

public partial class MultipleTooltip : Button
{
    private List<ITooltip> _activeTooltips = new List<ITooltip>();
    private const string tooltipIdMain = "tooltip_multiple";
    private const string tooltipId1 = "tooltip_multiple_top_right";
    private const string tooltipId2 = "tooltip_multiple_bottom_right";
    private const string tooltipId3 = "tooltip_multiple_top_left";
    private const string tooltipId4 = "tooltip_multiple_bottom_left";

    public override void _Ready()
    {
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExit;
    }

    private void OnMouseExit()
    {
        foreach (var activeTooltip in _activeTooltips)
        {
            TooltipService.ReleaseTooltip(activeTooltip);
        }
        _activeTooltips = new List<ITooltip>();
    }

    private void OnMouseEntered()
    {
        Godot.Vector2 screenSize = GetViewport().GetVisibleRect().Size;
        Godot.Vector2 position = GetScreenPosition();
        Godot.Vector2 position1 = new Godot.Vector2(screenSize.X, 0.0f);
        Godot.Vector2 position2 = new Godot.Vector2(screenSize.X, screenSize.Y);
        Godot.Vector2 position3 = new Godot.Vector2(0.0f, 0.0f);
        Godot.Vector2 position4 = new Godot.Vector2(0.0f, screenSize.Y);

        _activeTooltips.Add(TooltipService.ShowTooltipById(position, TooltipPivot.BottomLeft, tooltipIdMain));
        _activeTooltips.Add(TooltipService.ShowTooltipById(position1, TooltipPivot.TopRight, tooltipId1));
        _activeTooltips.Add(TooltipService.ShowTooltipById(position2, TooltipPivot.BottomRight, tooltipId2));
        _activeTooltips.Add(TooltipService.ShowTooltipById(position3, TooltipPivot.TopLeft, tooltipId3));
        _activeTooltips.Add(TooltipService.ShowTooltipById(position4, TooltipPivot.BottomLeft, tooltipId4));
    }
}
