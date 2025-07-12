using NestedTooltips;

public class Tooltip : ITooltip
{
    public ITooltip? Parent { get; private set; }
    public ITooltip? Child { get; private set; }
    public string Text => Control.Text;
    public Vector2 Position => Control.Position;

    public ITooltipControl Control { get; }

    public event Action? OnTooltipDestroyed;

    public Tooltip(ITooltipControl control)
    {
        ArgumentNullException.ThrowIfNull(control);
        Control = control;
    }

    public void ForceDestroy()
    {
        OnTooltipDestroyed?.Invoke();
        (Control as Node)?.QueueFree();
    }

    public void SetReleasable(bool value)
    {
        if (value)
        {
            ForceDestroy();
        }
    }
}