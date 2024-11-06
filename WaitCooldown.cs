namespace Remote.Api;

public class WaitCooldown(TimeSpan cooldown)
{
    private readonly SemaphoreSlim _lock = new SemaphoreSlim(1);

    private DateTime nextTime = DateTime.Now;

    public async Task Wait(CancellationToken cancellationToken)
    {
        try
        {
            await _lock.WaitAsync(cancellationToken);
            TimeSpan delta = nextTime - DateTime.Now;
            if (delta.TotalSeconds > 0)
                await Task.Delay(delta, cancellationToken);
            nextTime = DateTime.Now.Add(cooldown);
        }
        finally
        {
            _lock.Release();
        }
    }
}
