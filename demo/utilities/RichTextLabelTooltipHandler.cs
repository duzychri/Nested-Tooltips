// Created by: Marcin Kuhnert

namespace NestedTooltips.DemoScene;

public partial class RichTextLabelTooltipHandler : RichTextLabel
{
    private ITooltip? _activeTooltip;

    public override void _Ready()
    {
        this.MetaHoverStarted += OnMetaHoverStarted;
        this.MetaHoverEnded += OnMetaHoverEnded;
        this.MetaClicked += OnMetaClicked;
    }

    private void OnMetaHoverStarted(Variant meta)
    {
        string tooltipId = meta.AsString();
        if (string.IsNullOrEmpty(tooltipId)) return;

        if (_activeTooltip != null)
        {
            TooltipService.ReleaseTooltip(_activeTooltip);
        }

        // Adjust position to avoid overlap with the cursor.
        Vector2 mousePosition = GetViewport().GetMousePosition() + new Vector2(10, -10);
        _activeTooltip = TooltipService.ShowTooltipById(mousePosition, TooltipPivot.BottomLeft, tooltipId);
    }

    private void OnMetaHoverEnded(Variant meta)
    {
        if (_activeTooltip != null)
        {
            TooltipService.ReleaseTooltip(_activeTooltip);
            _activeTooltip = null;
        }
    }

    private void OnMetaClicked(Variant meta)
    {
        if (_activeTooltip != null)
        {
            TooltipService.ActionLockTooltip(_activeTooltip);
        }
    }
}
