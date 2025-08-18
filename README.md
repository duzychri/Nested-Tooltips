## Introduction

This project is a plugin for Godot 4.4.1 that provides a customizable tooltip system written in C#. It allows developers to create, display, and manage tooltips with advanced features such as dynamic positioning, styling, and interaction handling. The system is designed to be flexible and extendable, making it suitable for a wide range of applications, from simple UI hints to complex nested tooltip structures.

Nested tooltips are a feature that enables tooltips to open additional child tooltips when interacting with specific elements, such as links or buttons, within the parent tooltip. This behavior allows for hierarchical or contextual information to be displayed seamlessly, enhancing the user experience.

## Setup

These are the steps you need to follow to set up the project on your local machine. The project was created using Windows and no guarantee is made that it will work on other platforms. It is likely that it will work on Linux and MacOS. Because it is written in C# won't currently work for the web export of Godot.

### Downloading the project

1. Clone the repository from git (https://git.rz.uni-augsburg.de/kuhnerma/esp-25-tool-tip-plugin)

### Installing prerequisites

1. You will need a C# Editor. I used Visual Studio 2022 to create this project. You can download it from the [Visual Studio download page](https://visualstudio.microsoft.com/downloads/).
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

The primary way to interact with the tooltip system is through the `TooltipService` class and its methods and properties, as well as the returned `ITooltip` instances.

### Creating a tooltip

To create a simple tooltip you can use the `ShowTooltip` method in the `TooltipService` class; Use the `text` parameter to set a bbcode marked up text for the created tooltip and use the `position` and `pivot` parameter to set the location that the tooltip should be placed at. If the optional `width` parameter is set then the tooltip will be that width and switch to allow automatic word wrap so it can dynamically adjust its height based on the text content. If the `width` parameter is not set then the tooltip's width will be determined by the text content.

``` C#

ITooltip tooltip = TooltipService.ShowTooltip(tooltipPosition, TooltipPivot.BottomLeft, tooltipText, width);

```

The **pivot** is used to determine in what direction the tooltip should be 'drawn' in relation to the position supplied by the user. The pivot is a point in the tooltip in the range of 0.0 to 1.0 in both x and y direction, where (0,0) is the top left corner and (1,1) is the bottom right corner of the tooltip. 

As an example: If you use `TooltipPivot.BottomLeft` then the bottom left corner of the tooltip will be placed at the position supplied by the user and the tooltip will then be drawn upwards and to the right from that position.

The other alternative to creating a tooltip is to use the `ShowTooltipById` method. This method works the same way as `ShowTooltip`, but instead of a `text` parameter it takes a `tooltipId` parameter. This id is used to load the text of the tooltip from the `ITooltipDataProvider` that you can supply by setting the `TooltipService.TooltipDataProvider` property. For more information about the `ITooltipDataProvider` interface  see the [Configuration] section below.

### Destroying a tooltip

If you want a tooltip to close again there are multiple options:

- If the tooltip is a nested tooltip then it will close automatically should the conditions for it to stay open no longer apply.
- If you have created a tooltip (for example by using the `ShowTooltip` method) then the `SetReleasable` method will tell the tooltip to close itself as soon as it can. This allows for pinning and nesting to happen, as a pinned tooltip will stay open (as long as the conditions for the pin remain) even when `SetReleasable` is called. *Use this instead of `ForceDestroy` whenever you can.*
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

``` C#
    TooltipService.ActionLockTooltip(tooltip);
```

## Configuration

The tooltip system can be configured in different ways. You can change the behaviour of how the user can interact with the tooltips using the `Settings` property in the `TooltipService` and you can adjust the style or visuals of the tooltip by creating you own 'prefab' and providing the service a path to it using the `TooltipPrefabPath` property. The `TooltipDataProvider` property can be set to an implementation of the `ITooltipDataProvider` interface to provide the tooltip system with data for tooltips that are created using the `ShowTooltipById` method.

### Settings

...

### Data provider

... 

### Tooltip prefab

...

## Credits

The padlock icon used for the default tooltip was created by [Lorc](https://lorcblog.blogspot.com/) from [Game-icons.net](https://game-icons.net/1x1/lorc/padlock.html) and is licensed under the [CC BY 3.0](https://creativecommons.org/licenses/by/3.0/) license.