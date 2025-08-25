// Created by: Marcin Kuhnert (created) & Christoph Duzy (refactor/bugfixes)

namespace NestedTooltips.DemoScene;

public partial class LargeTooltip : Button
{
    private ITooltip? _activeTooltip;
    private const string tooltipId = "tooltip_large";

    public override void _Ready()
    {
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
        Pressed += OnPressed;
    }

    private void OnPressed()
    {
        if (_activeTooltip != null)
        { TooltipService.ActionLockTooltip(_activeTooltip); }
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
        _activeTooltip = TooltipService.ShowTooltipById(position, TooltipPivot.BottomLeft, tooltipId, width: 300);
    }
}
