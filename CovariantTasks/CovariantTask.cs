namespace System.Threading.CovariantTasks;

internal class CovariantTask<T>(Task<T> task) : ICovariantTask<T>
{
    public async Task<object?> GetTask()
        => await task;
}