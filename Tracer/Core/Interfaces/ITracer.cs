namespace Core.Interfaces
{
	public interface ITracer<T>
	{
        // Call at the beginning of the measured method
        void StartTrace();

        // Call at the end of the measured method
        void StopTrace();

        // Get measurement result
        T GetTraceResult();
    }
}