namespace Unitflow.Features.Health;

public sealed class HealthService(ILogger<HealthService> logger) : IHealthService
{
    public HealthStatusDto GetStatus()
    {
        if (Random.Shared.NextDouble() < 0.5)
        {
            var reading = Random.Shared.Next(0, 100);
            logger.LogWarning("Unstable health reading detected: {Reading}", reading);
        }

        return new HealthStatusDto("ok");
    }
}
