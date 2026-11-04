# Sylnode

Sylnode is a Windows utility which can copy the content of the main screen to a secondary display.

Most of the source code was built with the help of a ChatGPT o3-mini session and Copilot using Visual Studio 2022 17.13.0 Preview 4.0.

> Note that the _.NET Desktop Development_ payload must be installed to be able to build WinForms applications with .NET.

## Using Sylnode with a Swiss French keyboard

Sylnode starts mirroring as soon as it is launched: the main screen is copied live to the second display.

Press <kbd>Ctrl</kbd>+<kbd>§</kbd> to switch between the two states:

- **Live**: the second display follows the main screen.
- **Frozen**: the second display keeps the last image and shows the banner _Sylnode - écran gelé_.

On a Swiss French keyboard, <kbd>§</kbd> is the key in the top left corner, below <kbd>Esc</kbd> and to the left of <kbd>1</kbd>; it also carries <kbd>°</kbd>. The shortcut works whichever application is in front.

> The shortcut is bound to a key position, not to a character: it is <kbd>§</kbd> only while the active Windows input language is _French (Switzerland)_. With another keyboard layout, the same key carries another character (<kbd>/</kbd> on a US keyboard).

If another application already uses <kbd>Ctrl</kbd>+<kbd>§</kbd>, Sylnode reports _Failed to register hotkey_ at startup and the shortcut does nothing; close that application and start Sylnode again.

Sylnode also hides itself while a PowerPoint window is active, and shows up again afterwards. Its icon in the notification area offers _Activer_, which brings its window back, and _Quitter_, which closes it.

## Building the single file executable

To build the single file executable, run `.\build.bat`. At the end of the build, the script opens the `distrib` folder where you should find the `Sylnode.exe` file.
