using Microsoft.Extensions.Configuration;
using yaspi.common;

namespace yaspi.test.xUnit;

public class YaspiTestBase {

    protected IConfiguration configuration;
    protected string connectionString;
    protected  IEventBus eventBus;
    protected YaspiTestBase()
    {
        configuration = ConfigurationFactory.GetConfiguration();
        eventBus = new EventBus(configuration, GetEventBusSubscriber());
        connectionString = configuration.GetConnectionString("DefaultConnection");
    }
    protected IEventBusSubscriber GetEventBusSubscriber()
    {
        return new TestEventBusSubscriber();
    }

    protected IEventBus GetEventBus()
    {
        return new EventBus(configuration, GetEventBusSubscriber());
    }

}