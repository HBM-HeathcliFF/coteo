using coteo.Domain;
using coteo.Domain.Enum;

public class TimedHostedService : IHostedService, IDisposable
{
    private DataManager _dataManager;
    private Timer? _timer = null;
    readonly IServiceScopeFactory _serviceScopeFactory;

    public TimedHostedService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    private void Initialize()
    {
        var scope = _serviceScopeFactory.CreateScope();
        _dataManager = scope.ServiceProvider.GetRequiredService<DataManager>();
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        Initialize();

        _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromMinutes(1));

        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        var orders = _dataManager.Orders.GetOrders();
        if (orders != null)
        {
            foreach (var order in orders)
            {
                if (order.Status != OrderStatus.Completed &&
                    order.Status != OrderStatus.CompletedNotOnTime &&
                    order.Status != OrderStatus.Canceled &&
                    order.Status != OrderStatus.NotOnTime)
                {
                    if (order.Deadline < DateTime.Now)
                    {
                        order.Status = OrderStatus.NotOnTime;
                        _dataManager.Orders.SaveOrder(order);
                    }
                }
            }

            _dataManager.SaveChanges();
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}