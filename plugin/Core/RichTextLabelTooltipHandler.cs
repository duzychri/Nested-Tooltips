namespace NestedTooltips.Components
{
	public partial class RichTextLabelTooltipHandler : RichTextLabel
	{
		private RichTextLabel _richTextLabel = null!;
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

			var mousePosition = GetViewport().GetMousePosition();
			_activeTooltip = TooltipService.ShowTooltipById(mousePosition, TooltipPivot.TopLeft, tooltipId);
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
}
