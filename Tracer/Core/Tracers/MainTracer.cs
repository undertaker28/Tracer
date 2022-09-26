using Core.Interfaces;
using Core.Models;
using System.Collections.Concurrent;

namespace Core.Tracers
{
	public class MainTracer : ITracer<TraceResult>
	{
		private ConcurrentDictionary<int, ThreadTracer> threadTracers { get; set; }

		public MainTracer()
		{
            threadTracers = new ConcurrentDictionary<int, ThreadTracer>();
		}

		public TraceResult GetTraceResult()
		{
			return new TraceResult(threadTracers.Select(t => t.Value.GetTraceResult()).ToList());
		}

		public void StartTrace()
		{
            var threadId = Thread.CurrentThread.ManagedThreadId;
            threadTracers.TryGetValue(threadId, out ThreadTracer threadTracer);

            if (threadTracer is null)
			{
				int currentThreadId = Thread.CurrentThread.ManagedThreadId;
                threadTracer = new ThreadTracer(currentThreadId);
                threadTracers.GetOrAdd(currentThreadId, _ => threadTracer);
            }

			threadTracer.StartTrace();
		}

		public void StopTrace()
		{
            var threadId = Thread.CurrentThread.ManagedThreadId;
            threadTracers.TryGetValue(threadId, out ThreadTracer threadTracer);
            threadTracer.StopTrace();
		}
	}
}