# ConsoleWrapper
ConsoleWrapper allows starting a command-line executable, redirecting its standard input, output and error streams, and managing the process. And of course, all of this is customisable. Want a window? No problem. Change the IO encoding? You can do that too.

## Features
* Management of executable process
* Redirection (or not, if you prefer) of IO streams, with optional encoding choice
* Notification (events + manual reset events) of important events
* Buffer handler for output and error streams

## Installation
To install ConsoleWrapper, add the [nuget package](https://www.nuget.org/packages/carlst99.ConsoleWrapper/) to your project. Alternatively, paste this command into the nuget command line:

> nuget install carlst99.ConsoleWrapper

or this command into the Visual Studio Package Manager Console

> Install-Package carlst99.ConsoleWrapper

If you want to build from source, you can do that too.

## Documentation
See the [wiki pages](https://github.com/carlst99/ConsoleWrapper/wiki)
