namespace System.Threading.CovariantTasks;

public interface ICovariantTask<out T>
{
    Task<object?> GetTask();
}
