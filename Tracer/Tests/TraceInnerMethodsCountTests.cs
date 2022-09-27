using Core.Interfaces;
using Core.Models;
using Core.Tracers;

namespace Tests;

[TestFixture]
public class TraceInnerMethodsCountTests
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
	public void MethodWithoutInnerMethods()
	{
        testMethods.M1();

		var traceResult = tracer.GetTraceResult();
		Assert.That(traceResult.ThreadTraceResults[0].MethodTraceResults[0].InnerMethodTraceResults.Count, Is.EqualTo(0));
	}

	[Test]
	public void MethodWithOneInnerMethod()
	{
        testMethods.M2();

		var traceResult = tracer.GetTraceResult();
		Assert.Multiple(() =>
		{
			Assert.That(traceResult.ThreadTraceResults[0].MethodTraceResults.Count, Is.EqualTo(2));
			Assert.That(traceResult.ThreadTraceResults[0].MethodTraceResults[0].InnerMethodTraceResults.Count, Is.EqualTo(1));
		});
	}

	[Test]
	public void MethodWithManyInnerMethods()
	{
        testMethods.M3();

		var traceResult = tracer.GetTraceResult();
		Assert.Multiple(() =>
		{
			Assert.That(traceResult.ThreadTraceResults[0].MethodTraceResults[0].InnerMethodTraceResults.Count, Is.EqualTo(3));
			Assert.That(traceResult.ThreadTraceResults[0].MethodTraceResults[0].InnerMethodTraceResults[0].InnerMethodTraceResults.Count, Is.EqualTo(0));
			Assert.That(traceResult.ThreadTraceResults[0].MethodTraceResults[0].InnerMethodTraceResults[1].InnerMethodTraceResults.Count, Is.EqualTo(1));
			Assert.That(traceResult.ThreadTraceResults[0].MethodTraceResults[0].InnerMethodTraceResults[2].InnerMethodTraceResults.Count, Is.EqualTo(1));
		});
	}
}