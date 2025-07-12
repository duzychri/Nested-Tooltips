namespace NestedTooltips;

/// <summary>
/// The Godot node that represents a tooltip in the UI.
/// </summary>
public partial class TooltipControl : Control, ITooltipControl
{
    [Export] private Control _fullContainer = null!;
    [Export] private RichTextLabel _textLabel = null!;

    #region Properties

    /// <inheritdoc/>
    public float MinimumWidth
    {
        get => _fullContainer.CustomMinimumSize.X;
        set
        {
            Vector2 size = _fullContainer.CustomMinimumSize;
            size.X = value;
            _fullContainer.CustomMinimumSize = size;
        }
    }

    /// <inheritdoc/>
    public string Text
    {
        get => _textLabel.Text ?? "";
        set => _textLabel.Text = value;
    }

    /// <inheritdoc/>
    public bool WrapText
    {
        get => _textLabel.AutowrapMode != TextServer.AutowrapMode.Off;
        set => _textLabel.AutowrapMode = value ? TextServer.AutowrapMode.WordSmart : TextServer.AutowrapMode.Off;
    }

    /// <inheritdoc/>
    public float PinProgress
    {
        get { GD.PrintErr("PinProgress is not implemented in TooltipVisual!"); return 0f; }
        set { GD.PrintErr("PinProgress is not implemented in TooltipVisual!"); }
    }

    /// <inheritdoc/>
    public bool IsInteractable
    {
        get { GD.PrintErr("IsInteractable is not implemented in TooltipVisual!"); return false; }
        set { GD.PrintErr("IsInteractable is not implemented in TooltipVisual!"); }
    }

    #endregion Properties

    #region Events

    /// <inheritdoc/>
    public event Action<Vector2, string>? OnLinkClicked;
    /// <inheritdoc/>
    public event Action<Vector2, string>? OnLinkHoveredEnd;
    /// <inheritdoc/>
    public event Action<Vector2, string>? OnLinkHoveredStart;

    #endregion Events

    #region Lifecycle Methods

    public override void _Ready()
    {
        _textLabel.MetaHoverStarted += OnMetaHoveredStart;
        _textLabel.MetaHoverEnded += OnMetaHoveredEnd;
        _textLabel.MetaClicked += OnMetaClicked;
    }

    #endregion Lifecycle Methods

    #region Callback Methods

    private void OnMetaHoveredStart(Variant meta)
    {
        if (IsInteractable == false)
        { return; }

        string tooltipTextId = meta.AsString();
        Vector2 mousePosition = GetViewport().GetMousePosition();
        OnLinkHoveredStart?.Invoke(mousePosition, tooltipTextId);
    }

    private void OnMetaHoveredEnd(Variant meta)
    {
        if (IsInteractable == false)
        { return; }

        string tooltipTextId = meta.AsString();
        Vector2 mousePosition = GetViewport().GetMousePosition();
        OnLinkHoveredEnd?.Invoke(mousePosition, tooltipTextId);
    }

    private void OnMetaClicked(Variant meta)
    {
        if (IsInteractable == false)
        { return; }

        string tooltipTextId = meta.AsString();
        Vector2 mousePosition = GetViewport().GetMousePosition();
        OnLinkClicked?.Invoke(mousePosition, tooltipTextId);
    }

    #endregion Callback Methods
}