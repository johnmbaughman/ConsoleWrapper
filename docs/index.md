# ConsoleWrapper Documentation

Welcome to the documentation for [ConsoleWrapper](https://github.com/FalconEye36/ConsoleWrapper)!

!!! info
	This documentation is a work-in-progress! It is nowhere near complete. Please feel free to contribute yourself!
	
## About
ConsoleWrapper is a wrapper for console applications, allowing them to be run headlessly with redirected IO. In other words, ConsoleWrapper provides all the implementation for starting a headless console, redirecting its standard input, output and error streams, and managing the process. And of course, all of this is customisable. Want a window? No problem. Change the IO encoding? You can do that too.

#### Features
* Management of console process
* Redirection (or not, if you prefer) of IO streams, with optional encoding choice
* Notification (events + manual reset events) of important events
* Buffer handler for output and error streams
* <s>Fully</s>Mostly unit tested :wink:

## Installation
To install ConsoleWrapper, simply add the [nuget package](https://www.nuget.org/packages/FalconEye36.ConsoleWrapper) with your favourite package manager. Alternatively, paste this command into the nuget command line:

> nuget install FalconEye36.ConsoleWrapper

or this command into the Visual Studio Package Manager Console

> Install-Package FalconEye36.ConsoleWrapper

If you want to build from source, you can do that too.