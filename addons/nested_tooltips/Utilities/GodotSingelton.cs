// Created by: Christoph Duzy

using Godot;

namespace NestedTooltips.Internals;

/// <summary>
/// Implements the singleton pattern for a Godot node.
/// </summary>
/// <remarks>
/// Add a prefab with a node that has this script to Autoload menu.
/// <see href="https://docs.godotengine.org/en/stable/tutorials/scripting/singletons_autoload.html">See the documentation here.</see>
/// </remarks>
/// <typeparam name="T">The type of the singleton node. Set the type of the node implementing this script here.</typeparam>
public partial class GodotSingelton<T> : Node where T : Node
{
    /// <summary>
    /// The instance of the singleton node.
    /// </summary>
    public static T Instance { get; private set; } = null!;

    /// <inheritdoc />
    public override void _Ready()
    {
        if (Instance != null)
        {
            GD.PrintErr($"An instance of {typeof(T).Name} already exists, but the _Ready method was called again.");
            return;
        }
        Instance = (T)(Node)this;
    }
}
