using Core.Interfaces;
using Core.Models;
using Core.Tracers;

namespace Tests;

[TestFixture]
public class TraceThreadTests
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
    public void SingleMethod_OneThread()
    {
        testMethods.M1();

        Assert.That(tracer.GetTraceResult().ThreadTraceResults.Count, Is.EqualTo(1));
    }

	[Test]
	public void TwoMethods_OneThread()
	{
        testMethods.M1();
        testMethods.M1();

		Assert.That(tracer.GetTraceResult().ThreadTraceResults.Count, Is.EqualTo(1));
	}

	[Test]
	public void TwoDifferentMethods_OneThread()
	{
        testMethods.M1();
        testMethods.M2();
		
		Assert.That(tracer.GetTraceResult().ThreadTraceResults.Count, Is.EqualTo(1));
	}

	[Test]
	public void TwoMethods_TwoThreads()
	{
		var thread1 = new Thread(testMethods.M1);
		var thread2 = new Thread(testMethods.M1);
		thread1.Start();
		thread2.Start();
		thread1.Join();
		thread2.Join();

		Assert.That(tracer.GetTraceResult().ThreadTraceResults.Count, Is.EqualTo(2));
	}
}