# Force Step - Visual Studio Extension 

[![Build status](https://ci.appveyor.com/api/projects/status/auuht1u7eyasg65u?svg=true)](https://ci.appveyor.com/api/projects/status/ebifkut73l56lqwl/branch/main?svg=true)
<img alt="License: MIT" src="https://img.shields.io/badge/License-MIT-success.svg" />
  <img alt="Version" src="https://img.shields.io/badge/version-1.0-success.svg?cacheSeconds=2592000" />


Step over and skip breakpoints.

Download this extension from the (VS Gallery). See the [changelog](CHANGELOG.md) for release notes.

## Features

Adds four new commands to Visual Studio, each representing navigation in a cardinal direction:

- **Debug: Force Step Over** - Step over a function call and ignore any breakpoints that would have been triggered. 
- **Debug: Force Step Out** - Step out of a function call and ignore any breakpoints that would have been triggered.
- **Debug: Force Step Into** - Step into a function call and ignore any breakpoints that would have been triggered.
- **Debug: Force Continue** - Continue program execution, ignoring breakpoints until the program exits, or until **Debug: Enable Saved Breakpoints** is executed: 
- **Debug: Disable Saved Breakpoints** - Disable all currently active breakpoints.
- **Debug: Enable Saved Breakpoints** - Re-enable breakpoints disabled by **Debug: Disable Saved Breakpoints** or **Debug: Force Continue**.

The commands are visible under the debug menu while paused on a breakpoint. They can also be found to the right of Visual Studio's normal step controls.

![](Media/ForceStepIcons.jpg "Force Step Icons")

## Usage

You can choose new shortcuts for the commands under **Tools -> Options -> Environment -> Keyboard**. Search for the command names (e.g., Debug.ForceStepOut)


## Contributing

Pull requests welcome. If you have any feedback or bugs to report, please open an issue on GitHub.
