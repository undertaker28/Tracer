using Core.Interfaces;
using Core.Models;
using System.Diagnostics;

namespace Core.Tracers
{
	internal class MethodTracer : ITracer<MethodTraceResult>
	{
		private readonly MethodInfo methodInfo;	
		private readonly int stackFrameNumber;

        private List<MethodTracer> innerMethodTracers { get; }
        private int nestingСounter = 0;
        private readonly Stopwatch stopwatch;

        public MethodTracer(int frameNumber = 3)
		{
            stopwatch = new Stopwatch();
            innerMethodTracers = new List<MethodTracer>();
            methodInfo = new MethodInfo();

            stackFrameNumber = frameNumber;
		}		

		public MethodTraceResult GetTraceResult()
		{
			if (nestingСounter != 0)
				throw new InvalidOperationException("Different amount of StartTrace and StopTrace");

            methodInfo.InnerMethodTraceResults = innerMethodTracers.Select(method => method.GetTraceResult()).ToList();

			var traceResult = new MethodTraceResult(methodInfo.MethodName, methodInfo.ClassName, methodInfo.ExecutionTime, methodInfo.InnerMethodTraceResults);

			return traceResult;
		}

		public void StartTrace()
		{
			if (nestingСounter == 0)
			{
				var method = new StackTrace().GetFrame(stackFrameNumber).GetMethod();

                methodInfo.MethodName = method.Name;
                methodInfo.ClassName = method.DeclaringType.Name;

                stopwatch.Start();
			}
			else if (nestingСounter == 1)
			{
				var tracer = new MethodTracer(stackFrameNumber + 1);
                innerMethodTracers.Add(tracer);
				tracer.StartTrace();
			}
			else if (nestingСounter > 1)
                innerMethodTracers.Last().StartTrace();
			else
				throw new InvalidOperationException("Different amount of StartTrace and StopTrace");

            nestingСounter++;
		}

		public void StopTrace()
		{		
			if (nestingСounter == 1)
			{
                stopwatch.Stop();
                methodInfo.ExecutionTime = stopwatch.ElapsedMilliseconds;
			}
			else if (nestingСounter > 1)
                innerMethodTracers.Last().StopTrace();
			else
				throw new InvalidOperationException("Different amount of StartTrace and StopTrace");

            nestingСounter--;
		}

		private class MethodInfo
		{
			public string MethodName { get; set; }
			public string ClassName { get; set; }
			public long ExecutionTime { get; set;  }
			public List<MethodTraceResult> InnerMethodTraceResults { get; set; }
		}
	}
}