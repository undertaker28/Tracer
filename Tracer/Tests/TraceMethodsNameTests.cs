using Core.Tracers;

namespace Tests;

[TestFixture]
public class TraceMethodsNameTests
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
	public void SingleMethod_NameM1()
	{
        testMethods.M1();

		Assert.That(tracer.GetTraceResult().ThreadTraceResults[0].MethodTraceResults[0].MethodName, Is.EqualTo("M1"));
	}

	[Test]
	public void SingleMethod_NameM2()
	{
        testMethods.M2();

		Assert.Multiple(() =>
		{
			Assert.That(tracer.GetTraceResult().ThreadTraceResults[0].MethodTraceResults[0].MethodName, Is.EqualTo("M2"));
			Assert.That(tracer.GetTraceResult().ThreadTraceResults[0].MethodTraceResults[0].InnerMethodTraceResults[0].MethodName, Is.EqualTo("M1"));
			Assert.That(tracer.GetTraceResult().ThreadTraceResults[0].MethodTraceResults[1].MethodName, Is.EqualTo("M2"));
			Assert.That(tracer.GetTraceResult().ThreadTraceResults[0].MethodTraceResults[1].InnerMethodTraceResults[0].MethodName, Is.EqualTo("M1"));
		});
	}
	
	[Test]
	public void SingleMethod_NameM3()
	{
        testMethods.M3();

		Assert.Multiple(() =>
		{
			Assert.That(tracer.GetTraceResult().ThreadTraceResults[0].MethodTraceResults[0].MethodName, Is.EqualTo("M3"));
			Assert.That(tracer.GetTraceResult().ThreadTraceResults[0].MethodTraceResults[0].InnerMethodTraceResults[0].MethodName, Is.EqualTo("M1"));
			Assert.That(tracer.GetTraceResult().ThreadTraceResults[0].MethodTraceResults[0].InnerMethodTraceResults[1].MethodName, Is.EqualTo("M2"));
			Assert.That(tracer.GetTraceResult().ThreadTraceResults[0].MethodTraceResults[0].InnerMethodTraceResults[1].InnerMethodTraceResults[0].MethodName, Is.EqualTo("M1"));
			Assert.That(tracer.GetTraceResult().ThreadTraceResults[0].MethodTraceResults[0].InnerMethodTraceResults[2].MethodName, Is.EqualTo("M2"));
			Assert.That(tracer.GetTraceResult().ThreadTraceResults[0].MethodTraceResults[0].InnerMethodTraceResults[2].InnerMethodTraceResults[0].MethodName, Is.EqualTo("M1"));
		});
	}

    [Test]
    public void ThwoMethodsInDifferentsThreads()
    {
        var thread1 = new Thread(testMethods.M2);
        var thread2 = new Thread(testMethods.M2);
        thread1.Start();
        thread2.Start();
        thread1.Join();
        thread2.Join();

        var traceResult = tracer.GetTraceResult();
        Assert.Multiple(() =>
        {
            Assert.That(tracer.GetTraceResult().ThreadTraceResults[0].MethodTraceResults[0].MethodName, Is.EqualTo("M2"));
            Assert.That(tracer.GetTraceResult().ThreadTraceResults[0].MethodTraceResults[0].InnerMethodTraceResults[0].MethodName, Is.EqualTo("M1"));
            Assert.That(tracer.GetTraceResult().ThreadTraceResults[0].MethodTraceResults[1].MethodName, Is.EqualTo("M2"));
            Assert.That(tracer.GetTraceResult().ThreadTraceResults[0].MethodTraceResults[1].InnerMethodTraceResults[0].MethodName, Is.EqualTo("M1"));
        });

        Assert.Multiple(() =>
        {
            Assert.That(tracer.GetTraceResult().ThreadTraceResults[1].MethodTraceResults[0].MethodName, Is.EqualTo("M2"));
            Assert.That(tracer.GetTraceResult().ThreadTraceResults[1].MethodTraceResults[0].InnerMethodTraceResults[0].MethodName, Is.EqualTo("M1"));
            Assert.That(tracer.GetTraceResult().ThreadTraceResults[1].MethodTraceResults[1].MethodName, Is.EqualTo("M2"));
            Assert.That(tracer.GetTraceResult().ThreadTraceResults[1].MethodTraceResults[1].InnerMethodTraceResults[0].MethodName, Is.EqualTo("M1"));
        });
    }
}