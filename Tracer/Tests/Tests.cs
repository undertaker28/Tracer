using Core.Tracers;

namespace Tests;

[TestFixture]
public class Tests
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