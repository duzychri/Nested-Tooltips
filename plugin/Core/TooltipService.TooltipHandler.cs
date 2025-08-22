namespace NestedTooltips;

public partial class TooltipService
{
    private class TooltipHandler
    {
        private readonly ITooltipControl _control;
        private readonly Vector2 _desiredPosition;
        private readonly TooltipPivot _desiredPivot;

        /// <summary>The amount of time that the tooltip has been open.</summary>
        private double _aliveTime = 0.0;
        /// <summary>The time that the cursor has not been hovering over the tooltip.</summary>
        private double _cursorAwayTime = 0.0;
        /// <summary>Indicates that the tooltip should be destroyed under the right conditions.</summary>
        private bool _queuedForRelease = false;
        /// <summary>Shows that the tooltip has been deleted and should not be processed anymore.</summary>
        private bool _isFreed = false;
        /// <summary>Indicates that the tooltip was locked by a user interaction.</summary>
        private bool _isActionLocked = false;
        /// <summary>The last size before it was changed.</summary>
        private Vector2 _lastSize = Vector2.Zero;

        public TooltipHandler? Parent { get; }
        public TooltipHandler? Child { get; private set; }

        public ITooltip Tooltip { get; private set; }
        public string Text { get => _control.Text; set => _control.Text = value; }
        public Vector2 Size { get => _control.Size; set => _control.Size = value; }

        /// <summary>Indicates if the tooltip is or was locked by a user interaction.</summary>
        private bool WasLocked
        {
            get => _wasLocked;
            set
            {
                _wasLocked = value;
                _control.IsInteractable = value;
            }
        }
        private bool _wasLocked = false;

        public TooltipHandler(ITooltipControl control, TooltipHandler? parent, Vector2 desiredPosition, TooltipPivot desiredPivot, int? width)
        {
            _control = control;
            Parent = parent;

            // Set the size and the desired location.
            _desiredPosition = desiredPosition;
            _desiredPivot = desiredPivot;
            _lastSize = _control.Size;
            UpdatePosition();

            _control.Visible = false;
            _control.IsInteractable = false;
            _control.OnLinkHoveredStart += OnLinkHoveredStart;
            _control.OnLinkHoveredEnd += OnLinkHoveredEnd;
            _control.OnLinkClicked += OnLinkClicked;

            // If the width is set then we want that to become the desired (min & max at the same time) width.
            if (width.HasValue)
            {
                _control.MinimumWidth = width.Value;
                _control.WrapText = true;
            }
            else
            {
                _control.MinimumWidth = 0;
                _control.WrapText = false;
            }

            // Set the readonly tooltip.
            Tooltip = new Tooltip(() => Child?.Tooltip, parent?.Tooltip, Text, _desiredPosition, _desiredPivot);
        }

        public void Release()
        {
            // Check if we are locked. If not, then we can just delete the tooltip immediately.
            if (WasLocked == false)
            {
                DestroyInternal();
                return;
            }

            _queuedForRelease = true;
        }

        public void ForceDestroy()
        {
            DestroyInternal();
        }

        public void OnThisClicked()
        {
            _isActionLocked = true;
        }

        #region Lifecycle Methods

        public void Process(double deltaTime)
        {
            // If the tooltip is already freed, we don't need to do anything.
            if (_isFreed)
            { return; }

            // Update how long the tooltip has been alive.
            _aliveTime += deltaTime;

            // Based on the show delay setting we delay the tooltip's visibility.
            bool isVisible = _aliveTime >= Settings.ShowDelay;
            _control.Visible = isVisible;

            // Update how long the cursor has been away from the tooltip.
            if (_control.IsCursorOverTooltip() || Child != null)
            {
                _cursorAwayTime = 0.0;
                _control.UnlockProgress = 0.0;
            }
            else if (_queuedForRelease)
            {
                _cursorAwayTime += deltaTime;
                _control.UnlockProgress = GetUnlockProgress();
            }

            // Check if we are locked.
            switch (Settings.LockMode)
            {
                case TooltipLockMode.TimerLock:
                    {
                        (bool isLocked, double progress) = IsLockedByTimerLock();
                        _control.LockProgress = progress;
                        WasLocked = WasLocked || isLocked;
                    }
                    break;
                case TooltipLockMode.ActionLock:
                    {
                        bool isLocked = IsLockedByActionLock();
                        WasLocked = WasLocked || isLocked;
                        _control.LockProgress = isLocked ? 1.0 : 0.0;
                    }
                    break;
                default:
                    throw new InvalidOperationException($"Unknown lock mode: {Settings.LockMode}");
            }

            // Handle the release logic.
            if (_queuedForRelease)
            {
                // If we want to release the tooltip multiple conditions have to be fullfilled:
                // 1. The cursor can't be over the tooltip.
                // 2. The cursor has been away from the tooltip for a certain amount of time (unlocked).
                // 3. There can't be a child tooltip that is currently open.

                bool isCursorOverTooltip = _control.IsCursorOverTooltip();
                bool isUnlocked = GetUnlockProgress() >= 1;
                bool noOpenChild = Child == null;

                bool canRelease = isCursorOverTooltip == false && isUnlocked && noOpenChild;

                if (canRelease)
                {
                    DestroyInternal();
                }
            }

            // The size of the tooltip gets calulated after rending we can't immediately set the position.
            // Because of this we set it in the process method. That way it should update when the size is recalculated.
            // This could be changed to only be called once, but for simplicity and because of time constraints we'll do it in this way for now.
            if (_control.Size != _lastSize)
            {
                _lastSize = _control.Size;
                UpdatePosition();
            }
        }

        #endregion Lifecycle Methods

        #region Locking Logic

        private double GetUnlockProgress()
        {
            double unlockProgress = Math.Clamp(_cursorAwayTime / Settings.UnlockDelay, 0.0, 1.0);
            return unlockProgress;
        }

        private (bool isLocked, double progress) IsLockedByTimerLock()
        {
            bool isLocked = _aliveTime >= Settings.LockDelay;
            double lockProgress = Math.Clamp(_aliveTime / Settings.LockDelay, 0.0, 1.0);
            return (isLocked, lockProgress);
        }

        private bool IsLockedByActionLock()
        {
            return _isActionLocked;
        }

        #endregion Locking Logic

        #region Nesting Logic

        private void OnLinkHoveredStart(Vector2 cursorPosition, string tooltipTextId)
        {
            // We don't care about doing anything if we aren't locked.
            if (WasLocked == false)
            { return; }

            // Do not create another nested tooltip while there's already one open.
            // The exiting child has to be destroyed first.
            // There can only be one.
            DestroyChild();

            // Get the text for the tooltip.
            TooltipData? tooltipData = _dataProvider.GetTooltipData(tooltipTextId);
            if (tooltipData == null)
            {
                GD.PushError($"No tooltip data found for id: {tooltipTextId}. No nested tooltip will be created because of this!", nameof(tooltipTextId));
                return;
            }

            // Create the tooltip and set its text.
            (Vector2 nestedPosition, TooltipPivot nestedPivot) = CalculateNestedTooltipLocation(cursorPosition);
            (TooltipHandler childHandler, ITooltip _) = CreateTooltip(nestedPosition, nestedPivot, tooltipData.DesiredWidth, this);
            childHandler.Text = tooltipData.Text;
            Child = childHandler;
        }

        private void OnLinkHoveredEnd(Vector2 cursorPosition, string tooltipTextId)
        {
            // If the tooltip has a child, we need to release it.
            Child?.Release();
        }

        private void OnLinkClicked(Vector2 cursorPosition, string tooltipTextId)
        {
            // We don't care about doing anything if we aren't locked.
            if (WasLocked == false)
            { return; }

            // We already hovered over the link, so we should have a child tooltip.
            // Make sure we do have one.
            if (Child == null)
            {
                GD.PushError($"No child tooltip found for link click with id: {tooltipTextId}. This is a bug and should not happen!");
                return;
            }

            // Send a message to the child that it was clicked and it will handle the locking itself.
            Child.OnThisClicked();
        }

        private void DestroyChild()
        {
            if (Child == null)
            { return; }

            Child.ForceDestroy();
            Child = null;
        }

        #endregion Nesting Logic

        #region Utility Methods

        private void UpdatePosition()
        {
            // Calculate the position of the tooltip.
            Vector2 placementPosition = CalculatePositionFromPivot(_desiredPosition, _desiredPivot, _control.Size);
            placementPosition = CalculateNewTooltipLocation(placementPosition, _control.Size);

            _control.Position = placementPosition;
        }

        private void DestroyInternal()
        {
            // Do nothing if we already queued free.
            if (_isFreed)
            { return; }

            // Destroy the Godot node.
            _isFreed = true;
            _control.QueueFree();

            // Destroy any children.
            if (Child != null)
            {
                Child.ForceDestroy();
                Child = null;
            }

            // Tell the parent that we don't exist anymore.
            if (Parent != null)
            {
                Parent.Child = null;
            }

            // Make sure the service knows that this tooltip is gone.
            _destroyedTooltips.Add(Tooltip);
        }

        #endregion Utility Methods
    }
}