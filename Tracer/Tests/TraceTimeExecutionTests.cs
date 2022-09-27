using Core.Tracers;

namespace Tests;

[TestFixture]
public class TraceTimeExecutionTests
{
	private TestMethods testMethods;
	private MainTracer tracer;

	[SetUp]
	public void Setup()
	{
        tracer = new MainTracer();
        testMethods = new TestMethods(tracer);
	}

	[Test]
	public void OneMethod_TimeNear100()
	{
        testMethods.M1();

		Assert.That(tracer.GetTraceResult().ThreadTraceResults[0].ExecutionTime, Is.InRange(100, 200));
	}

	[Test]
	public void OneMethodWithInner_TimeNear400()
	{
        testMethods.M2();

		var traceResult = tracer.GetTraceResult();
		Assert.Multiple(() =>
		{
			Assert.That(traceResult.ThreadTraceResults[0].ExecutionTime, Is.InRange(400, 500));
			Assert.That(traceResult.ThreadTraceResults[0].MethodTraceResults[0].ExecutionTime, Is.InRange(200, 300));
			Assert.That(traceResult.ThreadTraceResults[0].MethodTraceResults[0].InnerMethodTraceResults[0].ExecutionTime, Is.InRange(100, 200));
			Assert.That(traceResult.ThreadTraceResults[0].MethodTraceResults[1].ExecutionTime, Is.InRange(200, 300));
			Assert.That(traceResult.ThreadTraceResults[0].MethodTraceResults[1].InnerMethodTraceResults[0].ExecutionTime, Is.InRange(100, 200));
		});
	}

	[Test]
	public void OneMethodWithManyInners_TimeNear500()
	{
        testMethods.M3();

		Assert.That(tracer.GetTraceResult().ThreadTraceResults[0].ExecutionTime, Is.InRange(500, 600));
	}

	[Test]
	public void ThwoMethodsInDifferentsThreads_TimeNear100()
	{
		var thread1 = new Thread(testMethods.M1);
		var thread2 = new Thread(testMethods.M1);
		thread1.Start();
		thread2.Start();
		thread1.Join();
		thread2.Join();

		var traceResult = tracer.GetTraceResult();
		Assert.Multiple(() =>
		{
			Assert.That(traceResult.ThreadTraceResults[0].ExecutionTime, Is.InRange(100, 200));
			Assert.That(traceResult.ThreadTraceResults[1].ExecutionTime, Is.InRange(100, 200));
		});
	}
}