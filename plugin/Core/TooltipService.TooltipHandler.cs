namespace NestedTooltips;

public partial class TooltipService
{
    private class TooltipHandler
    {
        private TooltipHandler? _child;
        private readonly TooltipHandler? _parent;
        private readonly ITooltipControl _control;

        /// <summary>The amount of time that the tooltip has been open.</summary>
        private double _aliveTime = 0.0;
        /// <summary>The time that the cursor has not been hovering over the tooltip.</summary>
        private double _cursorAwayTime = 0.0;
        /// <summary>Indicates if the tooltip is or was locked by a user interaction.</summary>
        private bool _wasLocked = false;
        /// <summary>Indicates that the tooltip should be destroyed under the right conditions.</summary>
        private bool _queuedForRelease = false;
        /// <summary>Shows that the tooltip has been deleted and should not be processed anymore.</summary>
        private bool _isFreed = false;

        public Tooltip Tooltip { get; private set; }
        public string Text { get => _control.Text; set => _control.Text = value; }
        public Vector2 Size { get => _control.Size; set => _control.Size = value; }
        public Vector2 Position { get => _control.Position; set => _control.Position = value; }

        public TooltipHandler(ITooltipControl control, TooltipHandler? parent)
        {
            _control = control;
            _parent = parent;

            Tooltip = new();

            _control.OnLinkHoveredStart += OnLinkHoveredStart;
            _control.OnLinkHoveredEnd += OnLinkHoveredEnd;
            _control.OnLinkClicked += OnLinkClicked;
        }

        public void Release()
        {
            // Check if we are locked. If not, then we can just delete the tooltip immediately.
            if (_wasLocked == false)
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

        #region Lifecycle Methods

        public void Process(double deltaTime)
        {
            // If the tooltip is already freed, we don't need to do anything.
            if (_isFreed)
            { return; }

            // Update how long the tooltip has been alive.
            _aliveTime += deltaTime;

            // Update how long the cursor has been away from the tooltip.
            if (_control.IsCursorOverTooltip() || _child != null)
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
                        _wasLocked = _wasLocked || isLocked;
                    }
                    break;
                case TooltipLockMode.ActionLock:
                    {
                        bool isLocked = IsLockedByActionLock();
                        _wasLocked = _wasLocked || isLocked;
                        _control.LockProgress = isLocked ? 1.0 : 0.0;
                    }
                    break;
                case TooltipLockMode.MouseTendency:
                    {
                        bool isLocked = IsLockedByMouseTendency();
                        _wasLocked = _wasLocked || isLocked;
                        _control.LockProgress = isLocked ? 1.0 : 0.0;
                    }
                    break;
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
                bool noOpenChild = _child == null;

                bool canRelease = isCursorOverTooltip == false && isUnlocked && noOpenChild;

                if (canRelease)
                {
                    DestroyInternal();
                }
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
            return true;
        }

        private bool IsLockedByMouseTendency()
        {
            // Check the movement of the mouse over the last few frames and determine if it is moving towards the tooltip.
            return true;
        }

        #endregion Locking Logic

        #region Nesting Logic

        private void OnLinkHoveredStart(Vector2 mousePosition, string tooltipTextId)
        {
            // We don't care about doing anything if we aren't locked.
            if (_wasLocked == false)
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
            (TooltipHandler childHandler, Tooltip _) = CreateTooltip(this);
            childHandler.Text = tooltipData.Text;
            _child = childHandler;

            // Calculate the position of the tooltip.
            TooltipPivot pivot = TooltipPivot.BottomCenter; // Default pivot for nested tooltips.
            Vector2 placementPosition = CalculateNewTooltipLocation(mousePosition, pivot, childHandler.Size);
            placementPosition = CalculatePositionFromPivot(placementPosition, pivot, childHandler.Size);
            childHandler.Position = placementPosition;
        }

        private void OnLinkHoveredEnd(Vector2 mousePosition, string tooltipTextId)
        {
            // If the tooltip has a child, we need to release it.
            if (_child != null)
            {
                _child.Release();
            }
        }

        private void OnLinkClicked(Vector2 mousePosition, string tooltipTextId)
        {
            // We don't care about doing anything if we aren't locked.
            if (_wasLocked == false)
            { return; }

            GD.Print($"TODO: TooltipService: OnLinkClicked called with {mousePosition}, {tooltipTextId}");
        }

        private void DestroyChild()
        {
            if (_child == null)
            { return; }

            _child.ForceDestroy();
            _child = null;
        }

        #endregion Nesting Logic

        #region Utility Methods

        private void DestroyInternal()
        {
            if (_isFreed)
            { return; }

            _isFreed = true;
            _control.QueueFree();
        }

        #endregion Utility Methods
    }
}