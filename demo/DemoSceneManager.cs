namespace NestedTooltips.DemoScene;

public partial class DemoSceneManager : Node
{
    [Export] private string _text = "Hello World!";

    public override void _Ready()
    {
        GD.Print(_text);
    }
}
