// This is the sample configuration for Rau.
// This uses C# syntax, and it is compiled and run when the program starts up.
//
// A real, production configuration file will most likely contain secrets,
// do not commit those to a public-facing repository.

// This is the section to define plugins.  These
// are dlls that are loaded into the process to help control Rau.
// This must be an absolute path.
// Does not need to be in quotes, any on the line that comes after #plugin
// will be included (including any comments, so do not put comments
// at the end of a line that starts with #plugin)
//
// #plugin can also be used to load any assembly the bot needs
// to function; an assembly does not need to include a compatible
// plugin in order to be loaded via #plugin.
#plugin /usr/lib/Rau/Plugins/Canary/Rau.Plugins.Canary.dll

/// <summary>
/// Configures the global settings for Rau.
/// This includes settings that are required to launch the service.
/// </summary>
public override void ConfigureRauSettings( IRauConfigurator rau )
{
    // 300 is Blue Sky's character limit.
    rau.SetCharacterLimit( 300 );

    // Uncomment and change the directory to enable logging to
    // a file.
    // rau.LogToFile( "/home/rau/rau.log" );

    // Comment out to not expose a Prometheus port.
    rau.UseMetricsAtPort( 9100 );

    // Uncomment to override the default user agent information that is used
    // when sending HTTP requests to a PDS.
    // rau.OverridePdsUserAgent( "my_bot", new Version( 1, 2, 3 ) );
}

/// <summary>
/// Configures the bot itself.  This method is run
/// after all plugins are loaded and initialized.
/// </summary>
public override void ConfigureBot( IRauApi rau )
{
    // For this example, it configures the canary plugin.
    // Each plugin will have its own implementation that you'll
    // have to include here.  Read your plugin's documentation
    // carefully to see how to configure it properly.

    rau.AddCanaryAccountWithDefaultMessage(
        new PdsAccount
        {
            UserName = "canary.bsky.social",
            Password = "Your App Password",
            Instance = new Uri( "https://bsky.social" )
        },
        // Chirp 13 minutes after the top-of every hour.
        "0 13 * * * ?"
    );
}
