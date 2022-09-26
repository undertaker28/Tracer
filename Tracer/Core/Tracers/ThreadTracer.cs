using Core.Interfaces;
using Core.Models;

namespace Core.Tracers
{
	internal class ThreadTracer : ITracer<ThreadTraceResult>
	{
        private List<MethodTracer> methodTracers { get; }

        private int nestingСounter = 0;
        private readonly int threadId;

        public ThreadTracer(int threadTracerId)
		{
            methodTracers = new List<MethodTracer>();
            threadId = threadTracerId;
		}	

		public ThreadTraceResult GetTraceResult()
		{
			if (nestingСounter != 0)
				throw new InvalidOperationException("Different amount of StartTrace and StopTrace");

			var threadTrace = new ThreadTraceResult(threadId,
                methodTracers.Select(t => t.GetTraceResult()).Sum(method => method.ExecutionTime),
                methodTracers.Select(t => t.GetTraceResult()).ToList());
			return threadTrace;
		}

		public void StartTrace()
		{
			if (nestingСounter == 0)
			{
				var methodTracer = new MethodTracer();
                methodTracers.Add(methodTracer);
			}
			else if (nestingСounter < 0)
				throw new InvalidOperationException("Different amount of StartTrace and StopTrace");

            nestingСounter++;

            methodTracers.Last().StartTrace();
		}

		public void StopTrace()
		{
			if (nestingСounter <= 0)
				throw new InvalidOperationException("Different amount of StartTrace and StopTrace");

            methodTracers.Last().StopTrace();
            nestingСounter--;
		}
	}
}