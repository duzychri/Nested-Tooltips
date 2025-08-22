namespace NestedTooltips;

/// <summary>
/// A default implementation of a tooltip control.
/// </summary>
public partial class TooltipControl : Control, ITooltipControl
{
    [ExportGroup("Base Components")]
    [Export] private Control _fullContainer = null!;
    [Export] private RichTextLabel _textLabel = null!;

    [ExportGroup("Lock Progress")]
    [Export] private TextureRect _lockIcon = null!;
    [Export] private TextureProgressBar _lockProgressBar = null!;

    private double _lockProgress = 0.0;
    private double _unlockProgress = 0.0;

    private const int RequiredMinimumWidth = 30;

    #region Properties

    /// <inheritdoc/>
    public float MinimumWidth
    {
        get => _fullContainer.CustomMinimumSize.X;
        set
        {
            Vector2 size = _fullContainer.CustomMinimumSize;
            size.X = Math.Max(RequiredMinimumWidth, value);
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
            UpdateLockingDisplay();
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
            UpdateLockingDisplay();
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

    /// <inheritdoc/>
    public override void _Ready()
    {
        _lockIcon.Visible = false;
        _lockProgressBar.Visible = false;

        _textLabel.MetaHoverStarted += OnMetaHoveredStart;
        _textLabel.MetaHoverEnded += OnMetaHoveredEnd;
        _textLabel.MetaClicked += OnMetaClicked;
    }

    #endregion Lifecycle Methods

    #region Other Methods

    /// <inheritdoc/>
    public bool IsCursorOverTooltip()
    {
        // Check if the mouse is currently over the tooltip control.
        Vector2 mousePosition = GetViewport().GetMousePosition();
        return GetGlobalRect().HasPoint(mousePosition);
    }

    private void UpdateLockingDisplay()
    {
        // Update lock progress, if we are locking.
        if (LockProgress > 0.0)
        {
            _lockIcon.Visible = LockProgress >= 1.0;
            _lockProgressBar.Visible = LockProgress > 0 && LockProgress < 1.0;
            _lockProgressBar.Value = LockProgress;
        }

        // If we are unlocking we overwrite the lock progress bar.
        if (UnlockProgress > 0.0)
        {
            _lockIcon.Visible = false;
            _lockProgressBar.Value = 1.0 - UnlockProgress;
            _lockProgressBar.Visible = UnlockProgress > 0 && UnlockProgress < 1.0;
        }

        // Make sure we don't show the lock stuff if it's not appropriate.
        if (LockProgress <= 0 && UnlockProgress <= 0)
        {
            _lockIcon.Visible = false;
            _lockProgressBar.Visible = false;
        }
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