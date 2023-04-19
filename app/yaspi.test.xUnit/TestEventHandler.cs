namespace yaspi.test.xUnit;

using Microsoft.Extensions.Configuration;
using yaspi.common;

public class TestEventHandler : IEventHandler<TestEvent> {
    public static string Message { get; set; }

    public TestEventHandler(IConfiguration configuration, IEventBus eventBus) {
        Message = string.Empty;
    }

    public void Handle(TestEvent @event) {
        Message = @event.Message;
    }
}