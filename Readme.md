# Rau

Rau is a framework that allows one to create bots to post to a PDS (personal data server) running on the AT protocol (AT-Proto).  Therefore, this can be used to write Blue Sky bots.

Don't be dick and use this for spamming.  Only make fun or useful bots with this framework.

## Configuration

Configuration of the bot is unique.  It is not a YAML or JSON or similar format, it is a custom C# configuration file that is compiled and run when the bot starts up.  See [RauConfig.cs](https://github.com/xforever1313/Rau/blob/main/Docker/rau/RauConfig.cs) inside of the ```Docker/rau``` directory of this repository for an example configuration.

When running Rau, this configuration file should be passed into Rau via any one of the following methods:

* Command line argument
  * Pass ```--config_file=<pathToFile>``` to your config file during startup.
* Environment Variable
  * Set the environment variable ```RAU_CONFIG_FILE``` to the path to the config file.
  * The command line argument takes priority over environment variables.
* Default location
  * The default location of the config file is in your user's roaming application data folder inside of a ```rau``` folder.  On Windows, this is ```C:\Users\<UserName>\AppData\Roaming\Rau\Config.cs```, and on Linux this is ```/home/<UserName>/.config/Rau/Config.cs```.  This file is used if no command line argument or environment variable is passed in.

To check to see if the config file will compile, pass in the ```--check_config``` command line argument when starting Rau.  If Rau exits with an exit code of 0, the config file's syntax is correct.

## Plugins

This bot can be extended by plugins.  The default Rau install comes with several default plugins you can use.

To implement your own plugin, you need a class that has a "RauPlugin" attribute that implements the IRauPlugin interface.  See a basic plugin example in Rau's [Canary Plugin](https://github.com/xforever1313/Rau/blob/main/src/Plugins/Rau.Plugins.Canary/CanaryPlugin.cs).

For more information on creating plugins, see our [Wiki page](https://github.com/xforever1313/Rau/wiki/Creating-Plugins) on it.

## Running

At the moment, the recommended method of running Rau is via Docker.  There is a Docker folder in the root of this repository you can copy that contains a docker-compose file.  Edit that and the RauConfig.cs file inside of the rau folder, and then run ```docker compose up -d```.  If you ever edit RauConfig.cs, you'll have to run ```docker compose up --force-recreate --build -d``` to ensure the new docker container takes your config.  Note, on some operating systems, the command may be ```docker-compose``` instead of ```docker compose```.

If you do not wish to install docker, you can download a zip file that is compatible with your operating system from the [releases](https://github.com/xforever1313/Rau/releases) section of Rau's GitHub page.  You will also need to install the Dotnet 8 ASP.NET Core Runtime.  Instructions on how to download it are on Microsoft's website [here](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).  From there, unzip the zip file and run ```./Rau --config_file=path/to/your/config/file.cs``` inside of the unzipped ```bin``` directory (```Rau.exe``` if on Windows).

There are plans to make a Windows Service and a Systemd file, but those are not implemented yet.

## Name

Rau is named after the Bionicle Kanohi mask [Rau](https://bionicle.fandom.com/wiki/Rau), which was the mask of translation.  The original intent of Rau was to get updates from a source (such as RSS) and translate them to the PDS format.
