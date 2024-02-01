namespace System.Threading.CovariantTasks;

public static class CovariantTaskUtils
{
    public static async Task<T> AsTask<T>(this ICovariantTask<T> task)
        => (T)(await task.GetTask())!;
    public static ICovariantTask<T> AsCovariant<T>(this Task<T> task)
        => new CovariantTask<T>(task);
}
