namespace NestedTooltips;

public class Tooltip : ITooltip
{
    public ITooltip? Parent { get; private set; }
    public ITooltip? Child { get; private set; }
    public string Text { get; private set; } = "";
    public Vector2 Position { get; private set; }

    public event Action? OnTooltipDestroyed;
}