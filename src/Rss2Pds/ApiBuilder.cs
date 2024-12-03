using Rau.Standard;

namespace Rau;

public abstract class ApiBuilder
{
    // ---------------- Constructor ----------------

    protected ApiBuilder()
    {
    }
    
    // ---------------- Methods ----------------
    
    public virtual void ConfigureRauSettings( IRauConfigurator configurator )
    {
    }

    public virtual void ConfigureBot( IRauApi rau )
    {
    }
}
