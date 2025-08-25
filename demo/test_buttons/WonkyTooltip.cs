// Created by: Marcin Kuhnert (created) & Christoph Duzy (refactor/bugfixes)

namespace NestedTooltips.DemoScene;

public partial class WonkyTooltip : Button
{
    private ITooltip? _activeTooltip;
    private const string tooltipId = "test_button_far_away";

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
        Vector2 position = new Vector2(0.0f, 0.0f);
        _activeTooltip = TooltipService.ShowTooltipById(position, TooltipPivot.BottomLeft, tooltipId);
    }
}
