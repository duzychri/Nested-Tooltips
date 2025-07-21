This is a small tooltip.

This is a large tooltip that gets a width provided when it is created leading to it line breaking to fit the specified width. This is unlike the small tooltip that only matches the length of the string but will not tline break unless those are manually added to the bbcode text as markup.

This is a tooltip with disabled (or rather not enabled nesting). The tooltip component is provided with this information and then determines itself how it should display links to other tooltips. It is also not interactable; mouse movement and events that hit this tooltip are ignored/passed through.

This is a normal nesting tooltip. Dependend on what settings you have chosen this tooltip can be pinned. Currently you have the time lock setting enabled, which means that this tooltip will be pinned after {LockTime} seconds of being open. After that has happened you can move your mouse into this tooltip to hover over links. Like this one!

This ui element spawns a tooltip in a wonky location. Can you even reach this befor the lock runs out and the tooltip disappears again?

This ui element spawns a tooltip and then just lets it sit there and never cleans it up. But you can press the button next to this to clear all tooltips that are currently open. Alternatively changing any setting will also close all currently open tooltips.

This ui element spawns multiple tooltips at the same time. These tooltips will be in all the corners of the screen. You can test how the different pin-settings interact with multiple open tooltips and how nested tooltips are spawned when you spawn them in invalid locations that would normally be outside the screen.