namespace NestedTooltips;

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