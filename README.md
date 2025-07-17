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

The idea of this plugin is to provide a clean and conscise way to create tooltips in Godot. The main class to interact with is the `ToolTipService`.

Its main interaction point is the `ShowTooltip` method, which can be used to display a tooltip at a given position with a given text. The `position` parameter deterines at which position on the screen the tooltip should be displayed. The `pivot` determines the direction that the tooltip is drawn from.  For example, if the pivot is set to `(0.0, 1.0)` (or the top left), then the tooltips top left corner will be at the supplied position. Assuming that the position is the cursors position, then the tooltip will be to the right and below the cursor. The `text` and `tooltipId` parameters are used to determine the content of the tooltip which can be bbcode formatted.

The `Settings` property of the `ToolTipService` can be used to configure the behaviour that is used in determining how and when the tooltip will be locked.

The tooltip itself also  provides two important methods `ForceDestroy` and `SetReleasable`:

- `ForceDestroy` will destroy the tooltip immediately, regardless of whether it is currently locked or not. It will also clean up all child tooltips.
- `SetReleasable` acxcepts a boolean value that determines whether the tooltip is allowed to destroy itself if it determines that it is no longer needed (like when the curser is no longer hovering over it or over a child tooltip).

## Configuration

The tooltip system can be configured by the developer in different ways. You can change the behaviour of how the user can interact with the tooltips using the `Settings` property in the `TooltipService` and you can adjust the style or visuals of the tooltip by creating you own 'prefab' and providing the service a path to it using the `TooltipPrefabPath` property.