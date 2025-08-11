namespace NestedTooltips;

/// <summary>
/// The Godot node that represents a tooltip in the UI.
/// </summary>
public partial class TooltipControl : Control, ITooltipControl
{
    [Export] private Label _debugLabel = null!;
    [Export] private Control _fullContainer = null!;
    [Export] private RichTextLabel _textLabel = null!;

    private double _lockProgress = 0.0;
    private double _unlockProgress = 0.0;

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
    public double LockProgress
    {
        get
        {
            return _lockProgress;
        }
        set
        {
            value = Math.Clamp(value, 0.0, 1.0);
            _lockProgress = value;
            UpdateDebugLabel();
        }
    }

    /// <inheritdoc/>
    public double UnlockProgress
    {
        get
        {
            return _unlockProgress;
        }
        set
        {
            value = Math.Clamp(value, 0.0, 1.0);
            _unlockProgress = value;
            UpdateDebugLabel();
        }
    }

    /// <inheritdoc/>
    public bool IsInteractable
    {
        get
        {
            return MouseFilter == MouseFilterEnum.Stop;
        }
        set
        {
            MouseFilter = value ? MouseFilterEnum.Stop : MouseFilterEnum.Ignore;
        }
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
        _debugLabel.Visible = false;

        _textLabel.MetaHoverStarted += OnMetaHoveredStart;
        _textLabel.MetaHoverEnded += OnMetaHoveredEnd;
        _textLabel.MetaClicked += OnMetaClicked;
    }

    #endregion Lifecycle Methods

    #region Other Methods

    public bool IsCursorOverTooltip()
    {
        // Check if the mouse is currently over the tooltip control.
        Vector2 mousePosition = GetViewport().GetMousePosition();
        return GetGlobalRect().HasPoint(mousePosition);
    }

    private void UpdateDebugLabel()
    {
        bool isVisible = LockProgress > 0.0 || UnlockProgress > 0.0;
        string text = $"Lock: {LockProgress * 100:000}% Unlock: {UnlockProgress * 100:000}%";
        _debugLabel.Visible = isVisible;
        _debugLabel.Text = text;
    }

    #endregion Other Methods

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