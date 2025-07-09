namespace NestedTooltips;

/// <summary>
/// The implementation of the tooltip component that can be nested within other tooltips.
/// </summary>
/// <remarks>
/// To provide a clean, concise API this should not be exposed to the user directly, but rather through the <see cref="ITooltipComponent"/> interface that limits interactions with useless Godot methods and properties.
/// </remarks>
public partial class TooltipComponent : Control, ITooltipComponent
{
    [Export] private Control _fullContainer = null!;
    [Export] private RichTextLabel _textLabel = null!;

    public ITooltipComponent? Parent { get; private set; }
    public ITooltipComponent? Child { get; private set; }
    public string Text { get => _textLabel.Text ?? ""; set => _textLabel.Text = value; }

    private int? _minWidth = null;

    /// <remarks>
    /// If set to <see langword="null"/> then autowrap will be turned off and the tooltip will be as long as the longest line of text. Use this for small tooltips, so you don't have to manually figure out the width of the tooltip.
    /// For longer tooltips it is recommended to supply a minimum size instead.
    /// </remarks>
    public int? MinWidth
    {
        get => _minWidth; set
        {
            _minWidth = value;
            if (value.HasValue)
            {
                Vector2 size = _fullContainer.Size;
                size.X = Math.Max(size.X, value.Value);
                _fullContainer.CustomMinimumSize = size;
                _textLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
            }
            else
            {
                Vector2 size = _fullContainer.Size;
                size.X = 0;
                _fullContainer.CustomMinimumSize = Vector2.Zero;
                _textLabel.AutowrapMode = TextServer.AutowrapMode.Off;
            }
        }
    }

    public event Action? OnTooltipDestroyed;

    public void ForceDestroy()
    {
        QueueFree();
    }

    public void SetReleasable(bool value)
    {
        QueueFree();
    }
}