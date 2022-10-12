using Core.Interfaces;
using Core.Models;
using System.Diagnostics;

namespace Core.Tracers
{
	internal class MethodTracer : ITracer<MethodTraceResult>
	{
		private readonly MethodInfo methodInfo;	
		private readonly int frameNum;

        private List<MethodTracer> innerMethodTracers { get; }
        private int nestingLevel = 0;
        private readonly Stopwatch stopwatch;

        public MethodTracer(int frameNumber = 3)
		{
            stopwatch = new Stopwatch();
            innerMethodTracers = new List<MethodTracer>();
            methodInfo = new MethodInfo();

            frameNum = frameNumber;
		}		

		public MethodTraceResult GetTraceResult()
		{
            methodInfo.InnerMethodTraceResults = innerMethodTracers.Select(method => method.GetTraceResult()).ToList();
			var traceResult = new MethodTraceResult(methodInfo.MethodName, methodInfo.ClassName, methodInfo.Time, methodInfo.InnerMethodTraceResults);
			return traceResult;
		}

		public void StartTrace()
		{
			if (nestingLevel == 0)
			{
				var method = new StackTrace().GetFrame(frameNum).GetMethod();
                methodInfo.MethodName = method.Name;
                methodInfo.ClassName = method.DeclaringType.Name;
                stopwatch.Start();
			}
			else if (nestingLevel == 1)
			{
				var tracer = new MethodTracer(frameNum + 1);
                innerMethodTracers.Add(tracer);
				tracer.StartTrace();
			}
			else if (nestingLevel > 1)
                innerMethodTracers.Last().StartTrace();

            nestingLevel++;
		}

		public void StopTrace()
		{		
			if (nestingLevel == 1)
			{
                stopwatch.Stop();
                methodInfo.Time = stopwatch.ElapsedMilliseconds;
			}
			else if (nestingLevel > 1)
                innerMethodTracers.Last().StopTrace();

            nestingLevel--;
		}

		private class MethodInfo
		{
			public string MethodName { get; set; }
			public string ClassName { get; set; }
			public long Time { get; set; }
			public List<MethodTraceResult> InnerMethodTraceResults { get; set; }
		}
	}
}