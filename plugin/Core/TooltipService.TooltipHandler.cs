namespace NestedTooltips;

public partial class TooltipService
{
    private static class MouseHelper
    {
        private const int MaxMousePositions = 10;
        private static Queue<(Vector2 position, double deltaTime)> _mousePositions = new();

        public static void Update(double deltaTime, Vector2 mousePosition)
        {
            // Add the current mouse position to the queue.
            _mousePositions.Enqueue((mousePosition, deltaTime));

            // If the queue exceeds the maximum size, remove the oldest position.
            while (_mousePositions.Count > MaxMousePositions)
            {
                _mousePositions.Dequeue();
            }
        }

        /// <summary>
        /// Returns the average mouse movement as a vector over a specified interval in seconds.
        /// </summary>
        public static Vector2 GetMouseTendency(double interval)
        {
            // Fallback to zero if there are no mouse positions recorded.
            if (_mousePositions.Count == 0)
            {
                return Vector2.Zero;
            }

            int count = 0;
            double time = 0.0;
            Vector2 movement = Vector2.Zero;

            // Iterate through the mouse positions and sum up all that happened in the interval.
            foreach ((Vector2 position, double deltaTime) in _mousePositions.Reverse())
            {
                count++;
                time += deltaTime;
                movement += position;

                // If the total time exceeds the interval, we can stop processing.
                if (time > interval)
                { break; }
            }

            // Calculate the average from the total movement.
            Vector2 averageMovement = movement / count;
            return averageMovement;
        }

        public static Vector2 GetCurrentMousePosition()
        {
            if (_mousePositions.Count == 0)
            {
                return Vector2.Zero;
            }

            Vector2 currentPosition = _mousePositions.Last().position;
            return currentPosition;
        }
    }

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
        /// <summary>Indicates that the tooltip was locked by a user interaction.</summary>
        private bool _isActionLocked = false;

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
                        (bool isLocked, double progress) = IsLockedByMouseTendency();
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
            return _isActionLocked;
        }

        private (bool isLocked, double progress) IsLockedByMouseTendency()
        {
            const double scannedInterval = 0.2; // The interval in seconds to check the mouse tendency.

            // Check the movement of the mouse over the last few frames and determine if it is moving towards the tooltip.
            // The tooltip is locked from mouse tendency if:
            // 1. The mouse is inside the tooltip.
            // 2. The tooltip is already locked and the mouse is not moving.
            // 3. The mouse is moving towards the tooltip.

            bool mouseIsOverTooltip = _control.IsCursorOverTooltip();

            // If the mouse is not over the tooltip, we are already locked and don't need to check further.
            if (mouseIsOverTooltip == false)
            { return (true, 1.0); }

            // Get the size and position of the tooltip.
            Vector2 tooltipSize = _control.Size;
            Vector2 tooltipPosition = _control.Position;

            // Get the current mouse position and the tendency of the mouse movement.
            Vector2 mousePosition = MouseHelper.GetCurrentMousePosition();
            Vector2 mouseTendency = MouseHelper.GetMouseTendency(scannedInterval);

            // Check if we are already locked and not moving (too much).
            bool notMoving = mouseTendency.Length() < 1f * scannedInterval;
            if (_wasLocked && notMoving)
            { return (true, 1.0); }

            // TODO: Actual behaviour.

            return (true, 1f);
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

            // We already hovered over the link, so we should have a child tooltip.
            // Make sure we do have one.
            if (_child == null)
            {
                GD.PushError($"No child tooltip found for link click with id: {tooltipTextId}. This is a bug and should not happen!");
                return;
            }

            // Send a message to the child that it was clicked and it will handle the locking itself.
            _child.OnThisClicked();
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