// Created by: Christoph Duzy

#if TOOLS
using Godot;
using System;

[Tool]
public partial class NestedTooltipsPlugin : EditorPlugin
{
    private const string Autoload_Prefab_Name = "TooltipServicePrefab";

    public override void _EnterTree()
    {
        AddAutoloadSingleton(Autoload_Prefab_Name, "res://addons/nested_tooltips/Prefabs/TooltipService_Prefab.tscn");
    }

    public override void _ExitTree()
    {
        RemoveAutoloadSingleton(Autoload_Prefab_Name);
    }
}
#endif
