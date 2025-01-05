# Rau RSS 2 PDS

This plugin can be used to mirror posts from an RSS feed and have them automatically be re-posted to an account in the PDS, and eventually make it to BlueSky.

This plugin is compatible with both [RSS](https://en.wikipedia.org/wiki/RSS) and [ATOM](https://en.wikipedia.org/wiki/Atom_(web_standard)) feeds.

## Live Example

The PDS at at.shendrick.net has a a few accounts that monitor Reddit's RSS feeds for [/r/csharp](https://old.reddit.com/r/csharp/) and [/r/dotnet](https://old.reddit.com/r/dotnet/).  They are listed below, and accessible via BlueSky:

* [/r/csharp](https://bsky.app/profile/r-csharp.at.shendrick.net)
* [/r/dotnet](https://bsky.app/profile/r-dotnet.at.shendrick.net)

## Configuration

In your RauConfig.cs file, you can configure which RSS feeds to monitor, and which accounts to re-post RSS items to.

```C#
#plugin /app/plugins/Rss2Pds/Rau.Plugins.Rss2Pds.dll

public override void ConfigureBot( IRauApi rau )
{
    rau.MirrorRssFeed(
        new FeedConfig
        {
            // This should point to the URL an RSS or ATOM file.
            FeedUrl = new Uri( "<RSS Feed Url>" ),
            // PDS user name to repost feed items to.
            UserName = "<Your User Name>",
            // Password of the PDS user.  It is strongly recommended that this
            // be an app password.
            Password = "<App Password>",
            // The URL of the PDS to use.
            PdsInstanceUrl = new Uri( "<PDS Url>" ),
            // How often to check the RSS feeds for updates using a cron string.
            // This string is every 20 minutes after the hour.
            // See https://www.quartz-scheduler.net/documentation/quartz-3.x/tutorial/crontriggers.html#cron-expressions for more information on how to make this string.
            CronString = "0 20 * * * ?",
            // List of hashtags to add to the end of the message sent to the PDS.
            // Set to null or empty array to not include any.
            HashTags = new string[] { "<Insert, or set to null>" },
            // How many failed attempts to get the feed in a row must happen before
            // and alert goes out to the admin of the bot.  Leave null to never
            // get an alert.
            AlertThreshold = 5,
            // Set to true to prefix the feed title in the post that goes to the PDS.
            // This can be useful if an account is mirroring multiple feeds.
            IncludeFeedTitleInPost = false,
            // If set to true, the initial GET of the feed occurs.  This can 
            // make startup time slightly slower.  Set to false if startup time is important;
            // but note that does mean the first check of the feed will only fill the bot's
            // cache, no messages will go out.
            InitializeOnStartUp = true,
            // The languages to set the posts to.
            // If left blank, and using an RSS feed, then the RSS feed's language will be used
            // instead.  If left blank and using an ATOM feed (or the language in the feed is not specified),
            // then the value specified in SetDefaultPostLanguages() during ConfigureRauSettings
            // will be used.
            // If SetDefaultPostLanguages() was never called, and there is not language
            // inside of the feed and this is set to empty or null, then the system language
            // will be used.  However, if using Docker that may not be specified,
            // and will result in errors.
            // tl;dr:
            // Set this if you know a feed does not include a language or
            // invoke SetDefaultPostLanguages() to cover all feeds that do not include a language.
            Languages = new string[] { "en", "en-US" }
        }
    );

    // ^ Repeat for every feed you want mirrored.
}
```

See [this file](https://github.com/xforever1313/Rau/blob/main/src/Plugins/Rau.Plugins.Rss2Pds/IRauApiExtensions.cs) for all available extension methods for this plugin.  See [FeedConfig.cs](https://github.com/xforever1313/Rau/blob/main/src/Plugins/Rau.Plugins.Rss2Pds/FeedConfig.cs) for more documentation on the FeedConfig object.
