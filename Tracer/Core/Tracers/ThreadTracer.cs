using Core.Interfaces;
using Core.Models;

namespace Core.Tracers
{
	internal class ThreadTracer : ITracer<ThreadTraceResult>
	{
        private List<MethodTracer> methodTracers { get; }

        private int nestingLevel = 0;
        private readonly int threadId;

        public ThreadTracer(int threadTracerId)
		{
            methodTracers = new List<MethodTracer>();
            threadId = threadTracerId;
		}	

		public ThreadTraceResult GetTraceResult()
		{
			var threadTrace = new ThreadTraceResult(threadId,
                methodTracers.Select(t => t.GetTraceResult()).Sum(method => method.ExecutionTime),
                methodTracers.Select(t => t.GetTraceResult()).ToList());
			return threadTrace;
		}

		public void StartTrace()
		{
			if (nestingLevel == 0)
			{
				var methodTracer = new MethodTracer();
                methodTracers.Add(methodTracer);
			}

            nestingLevel++;
            methodTracers.Last().StartTrace();
		}

		public void StopTrace()
		{
            methodTracers.Last().StopTrace();
            nestingLevel--;
		}
	}
}