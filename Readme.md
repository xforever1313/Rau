# Rau

Rau is a framework that allows one to create bots to post to a PDS (personal data server) running on the AT protocol (AT-Proto).  Therefore, this can be used to write Blue Sky bots.

Don't be dick and use this for spamming.  Only make fun or useful bots with this framework.

## Configuration

Configuration of the bot is unique.  It is not a YAML or JSON or similar format, it is a custom C# configuration file that is compiled when the bot starts up.
See Sample.Config.cs in the root of this repository to get started.

## Plugins

This bot can be extended by plugins.  The default Rau install comes with several default plugins you can use.

To implement your own plugin, you need a class that has a "RauPlugin" attribute that implements the IRauPlugin interface.
See a basic plugin example in Rau's [Canary Plugin](https://github.com/xforever1313/Rau/blob/main/src/Plugins/Rau.Plugins.Canary/CanaryPlugin.cs).

For more information on creating plugins, see our [Wiki page](https://github.com/xforever1313/Rau/wiki/Creating-Plugins) on it.

## Installation

TODO.

## Name

Rau is named after the Bionicle Kanohi mask [Rau](https://bionicle.fandom.com/wiki/Rau), which was the mask of translation.  The original intent of Rau was to get updates from a source (such as RSS) and translate them to the PDS format.
