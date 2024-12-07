# Rau

Rau is a framework that allows one to create bots to post to a PDS (personal data server) running on AT-Proto.  Therefore, ths can be used to write Blue Sky bots.

Don't be dick and use this for spamming.  Only make fun bots, useful bots with this framework.

## Configuration

Configuration of the bot is unique.  It is not a YAML or JSON or similar format, it is a custom C# configuration file
that is compiled when the bot starts up.
See Sample.Config.cs in the root of this repository to get started.

## Plugins

This bot can be extended by plugins.  The default Rau install comes with several default plugins you can use.

To implement your own plugin, you need a class that has a "RauPlugin" attribute that implements the IRauPlugin interface.
See a basic plugin example in Rau's [Canary Plugin](https://github.com/xforever1313/Rau/blob/main/src/Plugins/Rau.Plugins.Canary/CanaryPlugin.cs).

You can do the following to create a plugin:

* Make this git repository a sub-module and have your plugin csproj reference the [Rau.Standard](https://github.com/xforever1313/Rau/blob/main/src/Rau.Standard/Rau.Standard.csproj) project in this repo.
  * It is recommended to turn the [CopyLocal](https://learn.microsoft.com/en-us/dotnet/api/vslangproj.reference.copylocal?view=visualstudiosdk-2022) property to off since Rau.Standard.dll should already be installed on the target machine.
* Make a csproj that has a package reference to the Rau.Standard NuGet package.
  * It is recommended to set ["PrivateAssets" to "all"](https://learn.microsoft.com/en-us/dotnet/api/vslangproj.reference.copylocal?view=visualstudiosdk-2022) for the package reference in the .csproj since Rau.Standard.dll should already be installed on the target machine.
* If Rau is already installed on your machine, you can have a csproj reference the Rau.Standard.dll in the install location's bin directly.

## Installation

TODO.

## Name

Rau is named after the Bionicle Kanohi mask [Rau](https://bionicle.fandom.com/wiki/Rau), which was the mask of translation.  The original intent of Rau was to get updates from a source (such as RSS) and translate them to the PDS format.
