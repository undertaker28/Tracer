namespace Core.Models
{
	public class ThreadTraceResult
	{
		public ThreadTraceResult(int threadId, long time, IReadOnlyList<MethodTraceResult> methods)
		{
			Id = threadId;
			ExecutionTime = time;
			MethodTraceResults = methods;
		}

		public int Id { get; }
		public long ExecutionTime { get; }
		public IReadOnlyList<MethodTraceResult> MethodTraceResults { get; }
	}
}