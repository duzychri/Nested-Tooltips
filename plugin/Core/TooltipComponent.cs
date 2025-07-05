namespace NestedTooltips;

/// <summary>
/// The implementation of the tooltip component that can be nested within other tooltips.
/// </summary>
/// <remarks>
/// To provide a clean, concise API this should not be exposed to the user directly, but rather through the <see cref="ITooltipComponent"/> interface that limits interactions with useless Godot methods and properties.
/// </remarks>
public partial class TooltipComponent : Control, ITooltipComponent
{
    public ITooltipComponent? Parent { get; private set; }
    public ITooltipComponent? Child { get; private set; }
    public string Text { get; private set; } = string.Empty;


    public event Action? OnTooltipDestroyed;

    public void ForceDestroy()
    {
        GD.Print("TooltipComponent: ForceDestroy()");
    }

    public void SetReleasable(bool value)
    {
        GD.Print($"TooltipComponent: SetReleasable({value})");
    }
}