using static Godot.Range;

namespace NestedTooltips.DemoScene;

public partial class NumberedSlider : HBoxContainer
{
    [ExportCategory("Configuration")]
    [Export] private double _minValue = 0.0;
    [Export] private double _maxValue = 100.0;
    [Export] private double _step = 1.0;

    [ExportCategory("Components")]
    [Export] private HSlider _slider = null!;
    [Export] private Label _labelAmount = null!;

    public double Value
    {
        get => _slider.Value;
        set { _slider.Value = value; }
    }

    public event ValueChangedEventHandler ValueChanged
    {
        add => _slider.ValueChanged += value;
        remove => _slider.ValueChanged -= value;
    }

    public override void _Ready()
    {
        _slider.MinValue = _minValue;
        _slider.MaxValue = _maxValue;
        _slider.Step = _step;
        _slider.ValueChanged += OnSliderValueChanged;
    }

    private void OnSliderValueChanged(double value)
    {
        _labelAmount.Text = $"{value:0.00} sec";
    }
}
