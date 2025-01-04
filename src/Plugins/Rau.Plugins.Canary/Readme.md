# Rau Canary Plugin

This plugin can be used to send a post out to an account to the PDS every so often to show if the PDS is still up or not.  A use case could be you're scrolling through BlueSky and realize that you haven't seen a post from that account in a while.  It could be a sign that the PDS is down or having trouble.

## Live Example

The PDS at at.shendrick.net has a canary account at [@canary.at.shendrick.net](https://bsky.app/profile/canary.at.shendrick.net).

## Configuration

In your RauConfig.cs file, you can configure canary accounts in the ConfigureBot() method.  Remember to include the path to the plugin's dll.

```C#
#plugin /app/plugins/Canary/Rau.Plugins.Canary.dll

public override void ConfigureBot( IRauApi rau )
{
    rau.AddCanaryAccountWithDefaultMessage(
        new PdsAccount
        {
            UserName = "<Canary User Name>",
            Password = "<App Password>",
            Instance = new Uri( "<PDS Url>" )
        },
        // Chirp on the top-of every hour.
        "0 0 * * * ?"
    );
}
```

See [this file](https://github.com/xforever1313/Rau/blob/main/src/Plugins/Rau.Plugins.Canary/IRauApiExtensions.cs) for all available extension methods for this plugin.
