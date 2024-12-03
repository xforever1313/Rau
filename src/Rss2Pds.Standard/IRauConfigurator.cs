namespace Rau.Standard
{
    /// <summary>
    /// Interface that allows a user to configure
    /// Rau's global configuration.
    ///
    /// Plugin writers can write extension methods
    /// to this interface to allow configuration of their
    /// plugins during the configuration stage.
    /// </summary>
    public interface IRauConfigurator
    {
        // ---------------- Properties ----------------

        RauConfig Config { get; }

        // ---------------- Methods ----------------
        
        void Configure( RauConfig config );
    }
}