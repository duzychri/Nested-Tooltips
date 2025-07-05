namespace NestedTooltips;

/// <summary>
/// Determines the behavior of tooltips in the application.
/// </summary>
public record class TooltipSettings
{
    /// <summary>
    /// The delay before the tooltip is shown, in seconds.
    /// </summary>
    public float ShowDelay { get; init; }
    // Determines if a tooltip can be locked by the user.
    // This should be used on a tooltip by tooltip basis, as some tooltips may not need to be lockable.
    // So for now this get's ignored because it is not a global setting.
    // TODO: Delete this if it wasn't implemented by the end of the project.
    //public bool IsLockable { get; init; }

    /// <summary>
    /// The delay before the tooltip is locked while the cursor is in the source area, in seconds.
    /// </summary>
    public float LockDelay { get; init; }
    /// <summary>
    /// The delay before the tooltip is unlocked after the cursor leaves the tooltip area, in seconds.
    /// </summary>
    public float UnlockDelay { get; init; }
    /// <summary>
    /// The mode in which the tooltip can be locked.
    /// </summary>
    public TooltipLockMode LockMode { get; init; }

    public static TooltipSettings Default => new()
    {
        ShowDelay = 0.25f,

        LockDelay = 0.5f,
        UnlockDelay = 0.5f,
        LockMode = TooltipLockMode.TimerLock,
    };
}
