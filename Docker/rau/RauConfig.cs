// Sample Rau Config file.
// This uses C# syntax, and is compiled once at the startup of Rau.
// This means you have all the power of C# to make your configuration file.

// If you like environment variables, you should be able to get values out of them
// via C#'s Environment.GetEnvironmentVariable( string ) method.

// There is no need to add a "using" statement at the top.  That should
// be taken care of by plugins or Rau itself.

// Remember to delete the "Delete This" section below.

// Delete any plugins you do not want to use.

#plugin /app/plugins/Canary/Rau.Plugins.Canary.dll
#plugin /app/plugins/Rss2Pds/Rau.Plugins.Rss2Pds.dll

/// <summary>
/// Configures the global settings for Rau.
/// This includes settings that are required to launch the service.
/// </summary>
/// <remarks>
/// See https://github.com/xforever1313/Rau/blob/main/src/Rau/Configuration/IRauConfiguratorExtensions.cs
/// for all methods that can be called here.
/// </remarks>
public override void ConfigureRauSettings( IRauConfigurator rau )
{
    // 300 is Blue Sky's character limit.
    rau.SetCharacterLimit( 300 );

    // When running in Docker, you'll probably need to set this to whatever language you want
    // your posts to be in.
    // Without it, Rau will default to the system default, which may not be defined
    // in the Docker container.
    rau.SetDefaultPostLanguages( new string[] { "en", "en-US" } );

    rau.UsePersistenceDirectory( "/data/" );
    rau.UseMetricsAtPort( 9100 );

    // What messages get printed to the Console log (viewable via the "docker log" command).
    // Possible options:
    // RauLogLevel.Verbose
    // RauLogLevel.Debug
    // RauLogLevel.Information
    // RauLogLevel.Warning
    // RauLogLevel.Error
    // RauLogLevel.Fatal
    rau.SetConsoleLogLevel( RauLogLevel.Information );

    // Uncomment to send warnings and above messages to a Telegram chat.
    // rau.LogToTelegram( "<Telegram Bot Token>", "<Telegram Chat ID>" );
}

/// <summary>
/// Configures the bot itself.  This method is run
/// after all plugins are loaded and initialized.
/// </summary>
/// <remarks>
/// See https://github.com/xforever1313/Rau/blob/main/src/Rau.Standard/IRauApi.cs
/// to see all the methods and properties that can be invoked on the past in
/// API object.
/// 
/// View each plugin's documentation to see extensions that can be called as well.
/// </remarks>
public override void ConfigureBot( IRauApi rau )
{
    var pdsInstance = new Uri( "<Your PDS Instance>" );

    // -------- Canary Plugin --------

    // Example on how to use the Canary plugin.
    // Delete this entire section if not using the Canary plugin.

    rau.AddCanaryAccountWithDefaultMessage(
        new PdsAccount
        {
            UserName = "<Canary User Name>",
            Password = "<App Password>",
            Instance = pdsInstance
        },
        // Chirp on the top-of every hour.
        "0 0 * * * ?"
    );

    // -------- RSS Plugin --------

    // Example on how to use the RSS plugin.
    // Delete this entire section if not using the RSS plugin.

    var languages = new string[] { "en", "en-US" };

    rau.MirrorRssFeed(
        new FeedConfig
        {
            FeedUrl = new Uri( "<Rss Feed Url>" ),
            UserName = "<Your User Name>",
            Password = "<App Password>",
            PdsInstanceUrl = pdsInstance,
            // Every 20 minutes after the hour.
            CronString = "0 20 * * * ?",
            HashTags = new string[] { "<Insert, or set to null>" },
            AlertThreshold = 5,
            IncludeFeedTitleInPost = false,
            InitializeOnStartUp = true,
            Languages = languages
        }
    );

    // -------- Delete This --------

    // Delete these next three lines.  This is here to make sure
    // a default configuration isn't being used, and someone actually edited the file.
    throw new NotImplementedException(
        "Default configuration detected, please edit the 'RauConfig.cs' file in the 'rau' folder.  Remember to delete this exception from that file (should be at the bottom of the file)."
    );
}
