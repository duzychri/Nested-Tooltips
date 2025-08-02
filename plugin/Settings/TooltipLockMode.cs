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
}
