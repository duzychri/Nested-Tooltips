## Introduction

## Setup

### Downloading the project

1. Clone the repository from git (https://git.rz.uni-augsburg.de/kuhnerma/esp-25-tool-tip-plugin)

### Installing prerequisites

1. You will need a C# Editor. I used Visual Studio 2022. You can download it from the [Visual Studio download page](https://visualstudio.microsoft.com/downloads/).
2. You will need .NET 8.0 SDK installed on your system. You can download it from the [.NET download page](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) or install it via the _Visual Studio Installer_ when installing Visual Studio.

### Installing Godot & Opening the project

1. Download [Godot 4.4.1 (stable)](https://godotengine.org/download/archive/4.4.1-stable/) for **Windows .NET** ([direct download link](https://github.com/godotengine/godot-builds/releases/download/4.4.1-stable/Godot_v4.4.1-stable_mono_win64.zip))
2. Extract the downloaded zip file to a folder of your choice
3. Click the `Godot_v4.4.1-stable_mono_win64.exe` file to run the Godot editor
4. Click on `📁 Import` in the top left below the Godot logo
5. Navigate to the folder where you cloned the repository and select the `project.godot` file
6. Click `Open` to import the project and either use the `☑ Edit now` toggle or open the project from the list

### Running the project

1. Navigate in the FileSystem (bottom left) to `res://Demo/Demo_Scene.tscn` and double-click it to open the demo scene
2. Click the `▶` button (in the top right corner) or press `F5` to play the demo scene (Note: `F5` will play the main scene set in the project settings, which is the demo scene in this case. `F6` will play the currently open scene which should now also be the demo scene)

## Usage

The idea of this plugin is to provide a clean and conscise way to create tooltips (with nesting functionality) in Godot.

### Creating a tooltip

To create a simple tooltip you can use the `ShowTooltip` method in the `TooltipService` class; Use the `text` parameter to set a bbcode marked up text for the created tooltip and use the `position` and `pivot` parameter to set the location that the tooltip should be placed at.

``` C#

ITooltip tooltip = TooltipService.ShowTooltip(tooltipPosition, TooltipPivot.BottomLeft, tooltipText);

```

For more information about the exact parameters check out [[...create extra file here]].

### Destroying a tooltip

If you want a tooltip to close again there are multiple options:

- If the tooltip is a nested tooltip then it will close automatically should the conditions for it to stay open no longer apply.
- If you have created a tooltip (for example by using the `ShowTooltip` method) then the `SetReleasable` method will tell the tooltip to close itself as soon as it can. This allows for pinning and nesting to happen, as a pinned tooltip will stay open (as long as the conditions for the pin remain) even when `SetReleasable` is called.
- `ForceDestroy` will immediatly destroy the tooltip. Even if it is pinned and should stay otherwise open.
- `ClearTooltips` will destroy all tooltips.

``` C#
    if (tooltip != null)
    {
        TooltipService.ReleaseTooltip(tooltip);
        tooltip = null;
    }
```

### Pinning & Nesting

Tooltips can be *pinned*; doing so stops the tooltip from disappearing when the `ReleaseTooltip` method is called. The primary use if this is to enable nesting behaviours. If the `TimerLock` lock mode is set, then the pin will happen after the specified `LockDelay` in the settings. If the `ActionLock` lock mode is set, then a mouse click on the link inside a tooltip will pin the child tooltip opened from hovering over the link. If you have opened the tooltip yourself (e.g. by using the `ShowTooltip` method) then you have to supply the information that the tooltip should be pinned. Do so by using the `ActionLockTooltip` method in the `TooltipService`.


## Configuration

The tooltip system can be configured in different ways. You can change the behaviour of how the user can interact with the tooltips using the `Settings` property in the `TooltipService` and you can adjust the style or visuals of the tooltip by creating you own 'prefab' and providing the service a path to it using the `TooltipPrefabPath` property.