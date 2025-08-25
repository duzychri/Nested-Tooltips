// Created by: Marcin Kuhnert (created) & Christoph Duzy (refactor/bugfixes)

namespace NestedTooltips.DemoScene;

public partial class MultipleTooltip : Button
{
    private const string TooltipIdMain = "tooltip_multiple";
    private const string TooltipIdTopRight = "tooltip_multiple_top_right";
    private const string TooltipIdBottomRight = "tooltip_multiple_bottom_right";
    private const string TooltipIdTopLeft = "tooltip_multiple_top_left";
    private const string TooltipIdBottomLeft = "tooltip_multiple_bottom_left";

    private readonly List<ITooltip> _activeTooltips = [];

    public override void _Ready()
    {
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
        Pressed += OnPressed;
    }

    private void OnMouseExited()
    {
        foreach (ITooltip activeTooltip in _activeTooltips)
        {
            TooltipService.ReleaseTooltip(activeTooltip);
        }
        _activeTooltips.Clear();
    }

    private void OnMouseEntered()
    {
        Vector2 screenSize = GetViewport().GetVisibleRect().Size;
        Vector2 buttonPosition = GetScreenPosition();

        // Predefined tooltip positions
        Vector2 positionTopRight = new Vector2(screenSize.X, 0.0f);
        Vector2 positionBottomRight = new Vector2(screenSize.X, screenSize.Y);
        Vector2 positionTopLeft = new Vector2(0.0f, 0.0f);
        Vector2 positionBottomLeft = new Vector2(0.0f, screenSize.Y);

        // Display tooltips
        _activeTooltips.Add(TooltipService.ShowTooltipById(buttonPosition, TooltipPivot.BottomLeft, TooltipIdMain));
        _activeTooltips.Add(TooltipService.ShowTooltipById(positionTopRight, TooltipPivot.TopRight, TooltipIdTopRight));
        _activeTooltips.Add(TooltipService.ShowTooltipById(positionBottomRight, TooltipPivot.BottomRight, TooltipIdBottomRight));
        _activeTooltips.Add(TooltipService.ShowTooltipById(positionTopLeft, TooltipPivot.TopLeft, TooltipIdTopLeft));
        _activeTooltips.Add(TooltipService.ShowTooltipById(positionBottomLeft, TooltipPivot.BottomLeft, TooltipIdBottomLeft));
    }

    private void OnPressed()
    {
        foreach (ITooltip activeTooltip in _activeTooltips)
        {
            TooltipService.ActionLockTooltip(activeTooltip);
        }
    }
}
