namespace Unitflow.Features.Health;

public interface IHealthService
{
    HealthStatusDto GetStatus();
}
