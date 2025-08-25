// Created by: Christoph Duzy

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
