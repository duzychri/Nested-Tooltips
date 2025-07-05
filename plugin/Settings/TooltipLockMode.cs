namespace NestedTooltips;

/// <summary>
/// The setting that determines in what way the tooltip can be locked.
/// </summary>
public enum TooltipLockMode
{
    /// <summary>
    /// The tooltip is locked after it has stayed open for a certain amount of time.
    /// </summary>
	TimerLock,
    /// <summary>
    /// The tooltip is locked after a user interaction is done (button is pressed).
    /// </summary>
	ActionLock,
    /// <summary>
    /// The tooltip is locked based on the cursor position relative to the tooltip. (When the user moves the mouse towards the tooltip, it will lock, but moving away will unlock it.)
    /// </summary>
	MouseTendency,
}
